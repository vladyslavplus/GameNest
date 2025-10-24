using AutoMapper;
using GameNest.Grpc.OrderItems;
using GameNest.OrderService.BLL.Services.Interfaces;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace GameNest.OrderService.GrpcServer.Services
{
    public class OrderItemGrpcServiceImpl : OrderItemGrpcService.OrderItemGrpcServiceBase
    {
        private readonly IOrderItemService _orderItemService;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderItemGrpcServiceImpl> _logger;

        public OrderItemGrpcServiceImpl(
            IOrderItemService orderItemService,
            IMapper mapper,
            ILogger<OrderItemGrpcServiceImpl> logger)
        {
            _orderItemService = orderItemService;
            _mapper = mapper;
            _logger = logger;
        }

        public override async Task<OrderItemsResponse> GetAllOrderItems(
            GetAllOrderItemsRequest request,
            ServerCallContext context)
        {
            try
            {
                var items = await _orderItemService.GetAllAsync(context.CancellationToken);

                var response = new OrderItemsResponse();
                response.Items.AddRange(items.Select(i => _mapper.Map<OrderItem>(i)));

                _logger.LogInformation("Returned {Count} order items via gRPC", response.Items.Count);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllOrderItems gRPC call");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public override async Task<OrderItemResponse> GetOrderItemById(
            GetOrderItemByIdRequest request,
            ServerCallContext context)
        {
            try
            {
                if (!Guid.TryParse(request.Id, out var itemId))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid order item ID format"));
                }

                var itemDto = await _orderItemService.GetByIdAsync(itemId, context.CancellationToken);

                var response = new OrderItemResponse
                {
                    OrderItem = _mapper.Map<OrderItem>(itemDto)
                };

                _logger.LogInformation("Returned order item {ItemId} via gRPC", itemId);
                return response;
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetOrderItemById gRPC call");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public override async Task<OrderItemsResponse> GetOrderItemsByOrderId(
            GetOrderItemsByOrderIdRequest request,
            ServerCallContext context)
        {
            try
            {
                if (!Guid.TryParse(request.OrderId, out var orderId))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid order ID format"));
                }

                var items = await _orderItemService.GetByOrderIdAsync(orderId, context.CancellationToken);

                var response = new OrderItemsResponse();
                response.Items.AddRange(items.Select(i => _mapper.Map<OrderItem>(i)));

                _logger.LogInformation("Returned {Count} items for order {OrderId} via gRPC", response.Items.Count, orderId);
                return response;
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetOrderItemsByOrderId gRPC call");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }
    }
}