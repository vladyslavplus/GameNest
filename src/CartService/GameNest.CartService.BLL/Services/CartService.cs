using AutoMapper;
using GameNest.CartService.BLL.DTOs;
using GameNest.CartService.BLL.Interfaces;
using GameNest.CartService.DAL.Interfaces;
using GameNest.CartService.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace GameNest.CartService.BLL.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CartService> _logger;

        public CartService(
            ICartRepository cartRepository,
            IMapper mapper,
            ILogger<CartService> logger)
        {
            _cartRepository = cartRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ShoppingCartDto> GetCartAsync(Guid userId)
        {
            var cart = await _cartRepository.GetCartAsync(userId);
            return _mapper.Map<ShoppingCartDto>(cart);
        }

        public async Task<ShoppingCartDto> AddOrUpdateItemAsync(Guid userId, CartItemAddDto itemDto)
        {
            var cart = await _cartRepository.GetCartAsync(userId);
            var item = _mapper.Map<ShoppingCartItem>(itemDto);

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == item.ProductId);

            if (existingItem != null)
            {
                _logger.LogInformation("Updating item {ProductId} in cart for user {UserId}. New quantity: {Quantity}",
                    item.ProductId, userId, item.Quantity);
                existingItem.Quantity = item.Quantity;
                existingItem.Price = item.Price;
            }
            else
            {
                _logger.LogInformation("Adding new item {ProductId} to cart for user {UserId}. Quantity: {Quantity}",
                    item.ProductId, userId, item.Quantity);
                cart.Items.Add(item);
            }

            var updatedCart = await _cartRepository.UpdateCartAsync(cart);
            return _mapper.Map<ShoppingCartDto>(updatedCart);
        }

        public async Task<ShoppingCartDto> RemoveItemAsync(Guid userId, Guid productId)
        {
            var cart = await _cartRepository.GetCartAsync(userId);
            var itemToRemove = cart.Items.FirstOrDefault(i => i.ProductId == productId);

            if (itemToRemove != null)
            {
                _logger.LogInformation("Removing item {ProductId} from cart for user {UserId}.",
                    productId, userId);
                cart.Items.Remove(itemToRemove);
                var updatedCart = await _cartRepository.UpdateCartAsync(cart);
                return _mapper.Map<ShoppingCartDto>(updatedCart);
            }

            _logger.LogWarning("Attempted to remove non-existent item {ProductId} from cart for user {UserId}.",
                productId, userId);

            return _mapper.Map<ShoppingCartDto>(cart);
        }

        public async Task ClearCartAsync(Guid userId)
        {
            _logger.LogInformation("Clearing cart for user {UserId}.", userId);
            await _cartRepository.DeleteCartAsync(userId);
        }
    }
}
