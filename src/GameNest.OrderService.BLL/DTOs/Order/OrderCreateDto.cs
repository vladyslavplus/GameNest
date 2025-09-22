using GameNest.OrderService.BLL.DTOs.OrderItem;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameNest.OrderService.BLL.DTOs.Order
{
    public class OrderCreateDto
    {
        public Guid Customer_Id { get; set; }
        public List<OrderItemCreateDto> Items { get; set; } = new();
    }
}
