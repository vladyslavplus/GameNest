namespace GameNest.OrderService.Domain.Entities
{
    public class PaymentRecord : BaseEntity
    {
        public Guid Order_Id { get; set; }
        public DateTime Payment_Date { get; set; } = DateTime.UtcNow;
        public string Method { get; set; } = null!;
        public decimal Amount { get; set; }
        public string Status { get; set; } = null!;
    }
}
