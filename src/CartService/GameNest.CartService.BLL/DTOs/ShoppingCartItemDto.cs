namespace GameNest.CartService.BLL.DTOs
{
    public class ShoppingCartItemDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
