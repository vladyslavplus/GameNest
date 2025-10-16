using GameNest.AggregatorService.Clients;
using GameNest.AggregatorService.DTOs.Aggregated;
using GameNest.AggregatorService.DTOs.Orders;
using System.Globalization;

namespace GameNest.AggregatorService.Services
{
    public class OrderAggregatorService
    {
        private readonly OrdersGrpcClient _ordersClient;
        private readonly OrderItemsGrpcClient _orderItemsClient;
        private readonly ILogger<OrderAggregatorService> _logger;

        public OrderAggregatorService(
            OrdersGrpcClient ordersClient,
            OrderItemsGrpcClient orderItemsClient,
            ILogger<OrderAggregatorService> logger)
        {
            _ordersClient = ordersClient;
            _orderItemsClient = orderItemsClient;
            _logger = logger;
        }

        public async Task<AggregatedOrderDto?> GetAggregatedOrderAsync(Guid orderId, CancellationToken ct)
        {
            var order = await _ordersClient.GetOrderByIdAsync(orderId.ToString(), ct);
            if (order == null)
            {
                _logger.LogWarning("Order {OrderId} not found", orderId);
                return null;
            }

            var items = await _orderItemsClient.GetOrderItemsByOrderIdAsync(orderId.ToString(), ct)
                        ?? new List<OrderItemDto>();

            var itemsList = items.ToList();
            var itemCount = itemsList.Count;

            return new AggregatedOrderDto
            {
                Id = Guid.Parse(order.Id),
                CustomerId = Guid.Parse(order.CustomerId),
                OrderDate = DateTime.Parse(order.OrderDate, CultureInfo.InvariantCulture),
                Status = order.Status,
                TotalAmount = (decimal)order.TotalAmount, 
                Items = itemsList,
                ItemCount = itemCount,
                PartialData = itemCount == 0,
                Warnings = itemCount == 0 ? new[] { "Order items not found" } : Array.Empty<string>(),
                ResponseTimestamp = DateTime.UtcNow,
                Summary = $"Order {order.Id}, {itemCount} items, Total: {order.TotalAmount:N2} ₴"
            };
        }
    }
}
