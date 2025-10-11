using GameNest.AggregatorService.DTOs.Orders;

namespace GameNest.AggregatorService.Clients
{
    public class OrdersClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<OrdersClient> _logger;

        public OrdersClient(HttpClient httpClient, ILogger<OrdersClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<OrderDto>?> GetAllOrdersAsync(CancellationToken ct)
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/orders", ct);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to fetch orders: {StatusCode}", response.StatusCode);
                    return null;
                }

                var orders = await response.Content.ReadFromJsonAsync<IEnumerable<OrderDto>>(ct);
                return orders;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching orders");
                return null;
            }
        }

        public async Task<OrderDto?> GetOrderByIdAsync(Guid id, CancellationToken ct)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/orders/{id}", ct);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to fetch order {OrderId}: {StatusCode}", id, response.StatusCode);
                    return null;
                }

                var order = await response.Content.ReadFromJsonAsync<OrderDto>(ct);
                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching order {OrderId}", id);
                return null;
            }
        }
    }
}