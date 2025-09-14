namespace GameNest.OrderService.BLL.DTOs.Order
{
    public class OrderCreateDto
    {
        public Guid Customer_Id { get; set; }
        public string Status { get; set; } = null!;
        public decimal Total_Amount { get; set; }
    }
}
