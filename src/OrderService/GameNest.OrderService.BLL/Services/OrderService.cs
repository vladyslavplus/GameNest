using AutoMapper;
using GameNest.OrderService.BLL.DTOs.Order;
using GameNest.OrderService.BLL.Services.Interfaces;
using GameNest.OrderService.DAL.UOW;
using GameNest.OrderService.Domain.Entities;
using GameNest.OrderService.Domain.Exceptions;
using GameNest.OrderService.GrpcClients.Clients.Interfaces;
using GameNest.Shared.Events.Orders;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GameNest.OrderService.BLL.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICartGrpcClient _cartClient;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public OrderService(
            IUnitOfWork unitOfWork,
            ICartGrpcClient cartClient,
            IMapper mapper,
            ILogger<OrderService> logger,
            IPublishEndpoint publishEndpoint)
        {
            _unitOfWork = unitOfWork;
            _cartClient = cartClient;
            _mapper = mapper;
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<OrderDto> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var order = await _unitOfWork.Orders!.GetWithItemsByIdAsync(id, ct);
            if (order == null)
            {
                _logger.LogWarning("Order with id {OrderId} not found.", id);
                throw new NotFoundException($"Order with id {id} not found.");
            }

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<IEnumerable<OrderDto>> GetAllAsync(CancellationToken ct = default)
        {
            var orders = await _unitOfWork.Orders!.GetAllWithItemsAsync(ct);
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<OrderDto> CreateAsync(Guid userId, OrderCreateDto dto, CancellationToken ct = default)
        {
            var cart = await _cartClient.GetCartByUserIdAsync(userId);
            if (cart == null || !cart.Items.Any())
                throw new ValidationException("Cannot create an order from an empty or non-existent cart.");

            var order = _mapper.Map<Order>(dto);
            order.Customer_Id = userId;
            order.Status = "Pending";
            order.Order_Date = DateTime.UtcNow;

            foreach (var item in cart.Items)
            {
                var orderItem = new OrderItem
                {
                    Product_Id = Guid.Parse(item.ProductId),
                    Product_Title = item.ProductTitle,
                    Quantity = item.Quantity,
                    Price = (decimal)item.Price
                };
                order.Items.Add(orderItem);
                order.Total_Amount += orderItem.Price * orderItem.Quantity;
            }

            _unitOfWork.BeginTransaction();
            try
            {
                var orderId = await _unitOfWork.Orders!.CreateAsync(order, ct);
                order.Id = orderId;

                foreach (var item in order.Items)
                {
                    item.Order_Id = orderId;
                    await _unitOfWork.OrderItems!.CreateAsync(item, ct);
                }

                await _unitOfWork.CommitAsync(ct);

                await _publishEndpoint.Publish(new OrderCreatedEvent
                {
                    OrderId = order.Id,
                    CustomerId = userId,
                    TotalAmount = order.Total_Amount,
                    Status = order.Status
                }, ct);

                _logger.LogInformation("Published OrderCreatedEvent for OrderId {OrderId}", order.Id);
            }
            catch
            {
                await _unitOfWork.RollbackAsync(ct);
                throw;
            }

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> UpdateAsync(Guid id, OrderUpdateDto dto, CancellationToken ct = default)
        {
            try
            {
                _unitOfWork.BeginTransaction();

                var order = await _unitOfWork.Orders!.GetByIdAsync(id, ct);
                if (order == null)
                    throw new NotFoundException($"Order with id {id} not found.");

                if (string.IsNullOrWhiteSpace(dto.Status))
                    throw new ValidationException("Status cannot be empty.");

                if (dto.Status == order.Status)
                    return _mapper.Map<OrderDto>(order);

                order.Status = dto.Status;
                order.Updated_At = DateTime.UtcNow;

                await _unitOfWork.Orders.UpdateAsync(order, ct);
                await _unitOfWork.CommitAsync(ct);

                return _mapper.Map<OrderDto>(order);
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync(ct);
                throw;
            }
        }

        public async Task DeleteAsync(Guid id, bool softDelete = true, CancellationToken ct = default)
        {
            try
            {
                _unitOfWork.BeginTransaction();

                var order = await _unitOfWork.Orders!.GetByIdAsync(id, ct);
                if (order == null)
                    throw new NotFoundException($"Order with id {id} not found.");

                if (softDelete)
                {
                    var items = await _unitOfWork.OrderItems!.GetByOrderIdAsync(id, ct);
                    foreach (var item in items)
                    {
                        await _unitOfWork.OrderItems.DeleteAsync(item.Id, softDelete, ct);
                    }
                }

                await _unitOfWork.Orders.DeleteAsync(order.Id, softDelete, ct);
                await _unitOfWork.CommitAsync(ct);
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync(ct);
                throw;
            }
        }
    }
}
