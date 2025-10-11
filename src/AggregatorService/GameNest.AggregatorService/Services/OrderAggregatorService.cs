using GameNest.AggregatorService.Clients;
using GameNest.AggregatorService.DTOs.Aggregated;
using GameNest.AggregatorService.DTOs.Orders;

namespace GameNest.AggregatorService.Services
{
    public class OrderAggregatorService
    {
        private readonly OrdersClient _ordersClient;
        private readonly OrderItemsClient _orderItemsClient;
        private readonly ILogger<OrderAggregatorService> _logger;

        public OrderAggregatorService(OrdersClient ordersClient, OrderItemsClient orderItemsClient, ILogger<OrderAggregatorService> logger)
        {
            _ordersClient = ordersClient;
            _orderItemsClient = orderItemsClient;
            _logger = logger;
        }

        public async Task<AggregatedOrderDto?> GetAggregatedOrderAsync(Guid orderId, CancellationToken ct)
        {
            var order = await _ordersClient.GetOrderByIdAsync(orderId, ct);
            if (order == null)
            {
                _logger.LogWarning("Order {OrderId} not found", orderId);
                return null;
            }

            var items = await _orderItemsClient.GetByOrderIdAsync(orderId, ct) ?? new List<OrderItemDto>();

            var itemsList = items.ToList();
            var itemCount = itemsList.Count;

            var dto = new AggregatedOrderDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                OrderDate = order.OrderDate,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                Items = itemsList,
                ItemCount = itemCount,
                PartialData = itemCount == 0,
                Warnings = itemCount == 0 ? new[] { "Order items not found" } : Array.Empty<string>(),
                ResponseTimestamp = DateTime.UtcNow,
                Summary = $"Order {order.Id}, {itemCount} items, Total: {order.TotalAmount:N2} ₴"
            };

            return dto;
        }
    }
}
