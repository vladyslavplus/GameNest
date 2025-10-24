using System.Text.Json.Serialization;

namespace GameNest.AggregatorService.DTOs.Orders
{
    public class OrderItemDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("order_Id")]
        public Guid OrderId { get; set; }

        [JsonPropertyName("product_Id")]
        public Guid ProductId { get; set; }
        [JsonPropertyName("product_title")]
        public string ProductTitle { get; set; } = null!;

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonIgnore]
        public decimal Total => Quantity * Price;
    }
}
