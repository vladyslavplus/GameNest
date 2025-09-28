namespace GameNest.OrderService.BLL.DTOs.Customer
{
    public class CustomerDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }
    }
}
