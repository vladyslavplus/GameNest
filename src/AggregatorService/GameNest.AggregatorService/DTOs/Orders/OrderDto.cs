using System.Text.Json.Serialization;

namespace GameNest.AggregatorService.DTOs.Orders
{
    public class OrderDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("customer_Id")]
        public Guid CustomerId { get; set; }

        [JsonPropertyName("order_Date")]
        public DateTime OrderDate { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = default!;

        [JsonPropertyName("total_Amount")]
        public decimal TotalAmount { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }
}
