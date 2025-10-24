namespace GameNest.Shared.Events.Orders
{
    public record OrderCreatedEvent
    {
        public Guid OrderId { get; init; }
        public Guid CustomerId { get; init; }
        public decimal TotalAmount { get; init; }
        public string Status { get; init; } = default!;
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    }
}
