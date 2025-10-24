using AutoMapper;
using GameNest.Grpc.Orders;
using GameNest.OrderService.BLL.Services.Interfaces;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace GameNest.OrderService.GrpcServer.Services
{
    public class OrderGrpcServiceImpl : OrderGrpcService.OrderGrpcServiceBase
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderGrpcServiceImpl> _logger;

        public OrderGrpcServiceImpl(
            IOrderService orderService,
            IMapper mapper,
            ILogger<OrderGrpcServiceImpl> logger)
        {
            _orderService = orderService;
            _mapper = mapper;
            _logger = logger;
        }

        public override async Task<OrdersResponse> GetAllOrders(
            GetAllOrdersRequest request,
            ServerCallContext context)
        {
            try
            {
                var orders = await _orderService.GetAllAsync(context.CancellationToken);

                var response = new OrdersResponse();
                response.Items.AddRange(orders.Select(o => _mapper.Map<Order>(o)));

                _logger.LogInformation("Returned {Count} orders via gRPC", response.Items.Count);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllOrders gRPC call");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public override async Task<OrderResponse> GetOrderById(
            GetOrderByIdRequest request,
            ServerCallContext context)
        {
            try
            {
                if (!Guid.TryParse(request.Id, out var orderId))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid order ID format"));
                }

                var orderDto = await _orderService.GetByIdAsync(orderId, context.CancellationToken);

                var response = new OrderResponse
                {
                    Order = _mapper.Map<Order>(orderDto)
                };

                _logger.LogInformation("Returned order {OrderId} via gRPC", orderId);
                return response;
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetOrderById gRPC call");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }
    }
}