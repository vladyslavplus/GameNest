using GameNest.AggregatorService.DTOs.Orders;

namespace GameNest.AggregatorService.Clients
{
    public class OrderItemsClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<OrderItemsClient> _logger;

        public OrderItemsClient(HttpClient httpClient, ILogger<OrderItemsClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<OrderItemDto>?> GetByOrderIdAsync(Guid orderId, CancellationToken ct)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/orders/orderitems/by-order/{orderId}", ct);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to fetch order items for order {OrderId}: {StatusCode}", orderId, response.StatusCode);
                    return null;
                }

                return await response.Content.ReadFromJsonAsync<IEnumerable<OrderItemDto>>(cancellationToken: ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching order items for {OrderId}", orderId);
                return null;
            }
        }
    }
}
