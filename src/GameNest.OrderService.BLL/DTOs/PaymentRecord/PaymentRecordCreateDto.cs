namespace GameNest.OrderService.BLL.DTOs.PaymentRecord
{
    public class PaymentRecordCreateDto
    {
        public Guid Order_Id { get; set; }
        public DateTime Payment_Date { get; set; } = DateTime.UtcNow;
        public string Method { get; set; } = null!;
        public decimal Amount { get; set; }
        public string Status { get; set; } = null!;
    }
}
