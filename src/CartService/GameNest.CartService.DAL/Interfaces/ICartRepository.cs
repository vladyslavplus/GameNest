using GameNest.CartService.Domain.Entities;

namespace GameNest.CartService.DAL.Interfaces
{
    public interface ICartRepository
    {
        Task<ShoppingCart> GetCartAsync(Guid userId);
        Task<ShoppingCart> UpdateCartAsync(ShoppingCart cart);
        Task DeleteCartAsync(Guid userId);
    }
}
