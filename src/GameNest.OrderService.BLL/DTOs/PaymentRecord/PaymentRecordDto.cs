namespace GameNest.OrderService.BLL.DTOs.PaymentRecord
{
    public class PaymentRecordDto
    {
        public Guid Id { get; set; }
        public Guid Order_Id { get; set; }
        public DateTime Payment_Date { get; set; }
        public string Method { get; set; } = null!;
        public decimal Amount { get; set; }
        public string Status { get; set; } = null!;
        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }
    }
}
