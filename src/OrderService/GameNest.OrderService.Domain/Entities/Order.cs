namespace GameNest.OrderService.Domain.Entities
{
    public class Order : BaseEntity
    {
        public Guid Customer_Id { get; set; }
        public DateTime Order_Date { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = null!;
        public decimal Total_Amount { get; set; }
        public string Country { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Street { get; set; } = null!;
        public string ZipCode { get; set; } = null!;
        public List<OrderItem> Items { get; set; } = new();
    }
}
