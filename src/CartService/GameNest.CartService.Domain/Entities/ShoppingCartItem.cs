namespace GameNest.CartService.Domain.Entities
{
    public class ShoppingCartItem
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
