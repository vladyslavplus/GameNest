namespace GameNest.OrderService.BLL.DTOs.OrderItem
{
    public class OrderItemCreateDto
    {
        public Guid Product_Id { get; set; }
        public int Quantity { get; set; }
    }
}
