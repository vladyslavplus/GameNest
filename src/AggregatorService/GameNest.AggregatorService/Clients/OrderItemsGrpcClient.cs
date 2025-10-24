using GameNest.AggregatorService.DTOs.Orders;
using GameNest.Grpc.OrderItems;
using Grpc.Core;

namespace GameNest.AggregatorService.Clients
{
    public class OrderItemsGrpcClient
    {
        private readonly OrderItemGrpcService.OrderItemGrpcServiceClient _client;
        private readonly ILogger<OrderItemsGrpcClient> _logger;

        public OrderItemsGrpcClient(
            OrderItemGrpcService.OrderItemGrpcServiceClient client,
            ILogger<OrderItemsGrpcClient> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<IEnumerable<OrderItemDto>?> GetAllOrderItemsAsync(CancellationToken ct)
        {
            try
            {
                var response = await _client.GetAllOrderItemsAsync(
                    new GetAllOrderItemsRequest(), cancellationToken: ct);

                return response.Items.Select(MapToDto);
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "gRPC error while fetching all order items");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all order items");
                return null;
            }
        }

        public async Task<OrderItemDto?> GetOrderItemByIdAsync(string id, CancellationToken ct)
        {
            try
            {
                var response = await _client.GetOrderItemByIdAsync(
                    new GetOrderItemByIdRequest { Id = id }, cancellationToken: ct);

                return MapToDto(response.OrderItem);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                _logger.LogWarning(ex, "Order item {ItemId} not found", id);
                return null;
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "gRPC error while fetching order item {ItemId}", id);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching order item {ItemId}", id);
                return null;
            }
        }

        public async Task<IEnumerable<OrderItemDto>?> GetOrderItemsByOrderIdAsync(string orderId, CancellationToken ct)
        {
            try
            {
                var response = await _client.GetOrderItemsByOrderIdAsync(
                    new GetOrderItemsByOrderIdRequest { OrderId = orderId }, cancellationToken: ct);

                return response.Items.Select(MapToDto);
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "gRPC error while fetching items for order {OrderId}", orderId);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching items for order {OrderId}", orderId);
                return null;
            }
        }

        private static OrderItemDto MapToDto(OrderItem grpcItem)
        {
            if (grpcItem == null) return null!;
            return new OrderItemDto
            {
                Id = Guid.TryParse(grpcItem.Id, out var id) ? id : Guid.Empty,
                OrderId = Guid.TryParse(grpcItem.OrderId, out var oid) ? oid : Guid.Empty,
                ProductId = Guid.TryParse(grpcItem.ProductId, out var pid) ? pid : Guid.Empty,
                ProductTitle = grpcItem.ProductTitle,
                Quantity = grpcItem.Quantity,
                Price = (decimal)grpcItem.Price
            };
        }
    }
}
