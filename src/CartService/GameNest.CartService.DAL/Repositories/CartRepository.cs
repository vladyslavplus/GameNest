using GameNest.CartService.DAL.Interfaces;
using GameNest.CartService.Domain.Entities;
using GameNest.ServiceDefaults.Redis;
using Microsoft.Extensions.Logging;

namespace GameNest.CartService.DAL.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly IRedisCacheService _redisCache;
        private readonly ILogger<CartRepository> _logger;

        private const string CartKeyPrefix = "cart:";
        private static readonly TimeSpan CartExpiration = TimeSpan.FromDays(14);

        public CartRepository(IRedisCacheService redisCache, ILogger<CartRepository> logger)
        {
            _redisCache = redisCache;
            _logger = logger;
        }

        private static string GetKey(Guid userId) => $"{CartKeyPrefix}{userId}";

        public async Task<ShoppingCart> GetCartAsync(Guid userId)
        {
            var key = GetKey(userId);

            var cart = await _redisCache.GetDataAsync<ShoppingCart>(key);

            if (cart == null)
            {
                _logger.LogInformation("No cart found for user {UserId}, creating new one.", userId);
                return new ShoppingCart(userId);
            }

            _logger.LogInformation("Cart found for user {UserId}.", userId);
            return cart;
        }

        public async Task<ShoppingCart> UpdateCartAsync(ShoppingCart cart)
        {
            var key = GetKey(cart.UserId);

            await _redisCache.SetDataAsync(key, cart, CartExpiration);

            _logger.LogInformation("Cart updated for user {UserId}.", cart.UserId);
            return cart;
        }

        public async Task DeleteCartAsync(Guid userId)
        {
            var key = GetKey(userId);

            await _redisCache.RemoveDataAsync(key);
            _logger.LogInformation("Cart deleted for user {UserId}.", userId);
        }
    }
}
