using GameNest.OrderService.BLL.DTOs.OrderItem;

namespace GameNest.OrderService.BLL.DTOs.Order
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public Guid Customer_Id { get; set; }
        public DateTime Order_Date { get; set; }
        public string Status { get; set; } = null!;
        public decimal Total_Amount { get; set; }
        public string Country { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Street { get; set; } = null!;
        public string ZipCode { get; set; } = null!;
        public DateTime Created_At { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }
}
