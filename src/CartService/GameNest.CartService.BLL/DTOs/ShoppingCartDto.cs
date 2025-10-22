namespace GameNest.CartService.BLL.DTOs
{
    public class ShoppingCartDto
    {
        public Guid UserId { get; set; }
        public List<ShoppingCartItemDto> Items { get; set; } = new();
        public decimal TotalAmount => Items.Sum(i => i.Price * i.Quantity);
    }
}
