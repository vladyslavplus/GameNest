namespace GameNest.OrderService.Domain.Entities
{
    public class Customer : BaseEntity
    {
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
