namespace GameNest.OrderService.BLL.DTOs.PaymentRecord
{
    public class PaymentRecordCreateDto
    {
        public Guid Order_Id { get; set; }
        public string Method { get; set; } = null!;
        public decimal Amount { get; set; }
    }
}
