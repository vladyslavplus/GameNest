using AutoMapper;
using GameNest.OrderService.BLL.DTOs.Order;
using GameNest.OrderService.BLL.Services.Interfaces;
using GameNest.OrderService.DAL.UOW;
using GameNest.OrderService.Domain.Entities;
using GameNest.OrderService.Domain.Exceptions;

namespace GameNest.OrderService.BLL.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<OrderDto> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var order = await _unitOfWork.Orders!.GetByIdAsync(id, ct);
            if (order == null)
                throw new NotFoundException($"Order with id {id} not found.");

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<IEnumerable<OrderDto>> GetAllAsync(CancellationToken ct = default)
        {
            var orders = await _unitOfWork.Orders!.GetAllAsync(ct);
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<OrderDto> CreateAsync(OrderCreateDto dto, CancellationToken ct = default)
        {
            var customer = await _unitOfWork.Customers!.GetByIdAsync(dto.Customer_Id, ct);
            if (customer == null)
                throw new ValidationException($"Customer with id {dto.Customer_Id} does not exist.");

            var order = new Order
            {
                Customer_Id = dto.Customer_Id,
                Status = "Pending",
                Total_Amount = 0m,
                Items = new List<OrderItem>()
            };

            var orderId = await _unitOfWork.Orders!.CreateAsync(order, ct);

            decimal total = 0;

            foreach (var itemDto in dto.Items)
            {
                var product = await _unitOfWork.Products!.GetByIdAsync(itemDto.Product_Id, ct);
                if (product == null)
                    throw new ValidationException($"Product with id {itemDto.Product_Id} does not exist.");

                var item = new OrderItem
                {
                    Order_Id = orderId,
                    Product_Id = product.Id,
                    Quantity = itemDto.Quantity,
                    Price = product.Price 
                };

                await _unitOfWork.OrderItems!.CreateAsync(item, ct);
                total += item.Price * item.Quantity;
            }

            order.Total_Amount = total;
            order.Id = orderId;
            await _unitOfWork.Orders.UpdateAsync(order, ct);

            await _unitOfWork.CommitAsync(ct);

            var created = await _unitOfWork.Orders.GetByIdAsync(orderId, ct);
            return _mapper.Map<OrderDto>(created);
        }

        public async Task<OrderDto> UpdateAsync(Guid id, OrderUpdateDto dto, CancellationToken ct = default)
        {
            var order = await _unitOfWork.Orders!.GetByIdAsync(id, ct);
            if (order == null)
                throw new NotFoundException($"Order with id {id} not found.");

            if (string.IsNullOrWhiteSpace(dto.Status))
                throw new ValidationException("Status cannot be empty.");

            if (dto.Status == order.Status)
                throw new ValidationException("No changes detected for the update.");

            order.Status = dto.Status;
            order.Updated_At = DateTime.UtcNow;

            await _unitOfWork.Orders.UpdateAsync(order, ct);
            await _unitOfWork.CommitAsync(ct);

            return _mapper.Map<OrderDto>(order);
        }

        public async Task DeleteAsync(Guid id, bool softDelete = true, CancellationToken ct = default)
        {
            var order = await _unitOfWork.Orders!.GetByIdAsync(id, ct);
            if (order == null)
                throw new NotFoundException($"Order with id {id} not found.");

            var items = await _unitOfWork.OrderItems!.GetByOrderIdAsync(id, ct);
            foreach (var item in items)
            {
                await _unitOfWork.OrderItems.DeleteAsync(item.Id, softDelete, ct);
            }

            await _unitOfWork.Orders.DeleteAsync(order.Id, softDelete, ct);
            await _unitOfWork.CommitAsync(ct);
        }
    }
}
