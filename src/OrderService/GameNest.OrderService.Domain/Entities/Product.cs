namespace GameNest.OrderService.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
    }
}
