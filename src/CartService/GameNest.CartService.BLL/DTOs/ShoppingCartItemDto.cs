namespace GameNest.CartService.BLL.DTOs
{
    public class ShoppingCartItemDto
    {
        public Guid ProductId { get; set; }
        public string ProductTitle { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
