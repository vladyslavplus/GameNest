using GameNest.Grpc.Orders;
using Grpc.Core;

namespace GameNest.AggregatorService.Clients
{
    public class OrdersGrpcClient
    {
        private readonly OrderGrpcService.OrderGrpcServiceClient _client;
        private readonly ILogger<OrdersGrpcClient> _logger;

        public OrdersGrpcClient(
            OrderGrpcService.OrderGrpcServiceClient client,
            ILogger<OrdersGrpcClient> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<IEnumerable<Order>?> GetAllOrdersAsync(CancellationToken ct)
        {
            try
            {
                var request = new GetAllOrdersRequest();

                var response = await _client.GetAllOrdersAsync(request, cancellationToken: ct);
                return response.Items;
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "gRPC error while fetching all orders");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching all orders");
                return null;
            }
        }

        public async Task<Order?> GetOrderByIdAsync(string orderId, CancellationToken ct)
        {
            try
            {
                var request = new GetOrderByIdRequest
                {
                    Id = orderId
                };

                var response = await _client.GetOrderByIdAsync(request, cancellationToken: ct);
                return response.Order;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                _logger.LogWarning(ex, "Order {OrderId} not found", orderId);
                return null;
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "gRPC error while fetching order {OrderId}", orderId);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching order {OrderId}", orderId);
                return null;
            }
        }
    }
}