namespace GameNest.OrderService.BLL.DTOs.OrderItem
{
    public class OrderItemDto
    {
        public Guid Id { get; set; }
        public Guid Order_Id { get; set; }
        public Guid Product_Id { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }
    }
}
