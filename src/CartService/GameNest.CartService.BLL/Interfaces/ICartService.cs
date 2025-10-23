using GameNest.CartService.BLL.DTOs;

namespace GameNest.CartService.BLL.Interfaces
{
    public interface ICartService
    {
        Task<ShoppingCartDto> GetCartAsync(Guid userId);
        Task<ShoppingCartDto> AddOrUpdateItemAsync(Guid userId, CartItemChangeDto itemDto);
        Task<ShoppingCartDto> RemoveItemAsync(Guid userId, Guid productId);
        Task ClearCartAsync(Guid userId);
    }
}
