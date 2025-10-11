using GameNest.AggregatorService.DTOs.Orders;

namespace GameNest.AggregatorService.DTOs.Aggregated
{
    public class AggregatedOrderDto : BaseAggregatedDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
        public int ItemCount { get; set; }
    }
}