namespace GameNest.OrderService.Domain.Entities
{
    public class OrderItem : BaseEntity
    {
        public Guid Order_Id { get; set; }
        public Guid Product_Id { get; set; }
        public string Product_Title { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
