namespace GameNest.OrderService.BLL.DTOs.Order
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public Guid Customer_Id { get; set; }
        public DateTime Order_Date { get; set; }
        public string Status { get; set; } = null!;
        public decimal Total_Amount { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }
    }
}
