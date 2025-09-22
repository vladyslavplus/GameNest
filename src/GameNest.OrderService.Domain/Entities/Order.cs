using System.ComponentModel.DataAnnotations.Schema;

namespace GameNest.OrderService.Domain.Entities
{
    public class Order : BaseEntity
    {
        public Guid Customer_Id { get; set; }
        public DateTime Order_Date { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = null!;
        public decimal Total_Amount { get; set; }
        [NotMapped]
        public List<OrderItem> Items { get; set; } = new();
    }
}
