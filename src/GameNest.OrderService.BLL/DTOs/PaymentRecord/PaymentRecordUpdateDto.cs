namespace GameNest.OrderService.BLL.DTOs.PaymentRecord
{
    public class PaymentRecordUpdateDto
    {
        public DateTime? Payment_Date { get; set; }
        public string? Method { get; set; }
        public decimal? Amount { get; set; }
        public string? Status { get; set; }
    }
}
