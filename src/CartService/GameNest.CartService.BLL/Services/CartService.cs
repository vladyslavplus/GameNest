using AutoMapper;
using GameNest.CartService.BLL.DTOs;
using GameNest.CartService.BLL.Interfaces;
using GameNest.CartService.DAL.Interfaces;
using GameNest.CartService.Domain.Entities;
using GameNest.CartService.Grpc.Clients.Interfaces;
using Microsoft.Extensions.Logging;

namespace GameNest.CartService.BLL.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CartService> _logger;
        private readonly IGameGrpcClient _gameClient;

        public CartService(
            ICartRepository cartRepository,
            IMapper mapper,
            ILogger<CartService> logger,
            IGameGrpcClient gameClient)
        {
            _cartRepository = cartRepository;
            _mapper = mapper;
            _logger = logger;
            _gameClient = gameClient;
        }

        public async Task<ShoppingCartDto> GetCartAsync(Guid userId)
        {
            var cart = await _cartRepository.GetCartAsync(userId);
            return _mapper.Map<ShoppingCartDto>(cart);
        }

        public async Task<ShoppingCartDto> AddOrUpdateItemAsync(Guid userId, CartItemChangeDto itemDto)
        {
            var game = await _gameClient.GetGameByIdAsync(itemDto.ProductId);
            if (game == null)
            {
                _logger.LogWarning("Game with ID {ProductId} not found when changing cart for user {UserId}", itemDto.ProductId, userId);
                throw new KeyNotFoundException($"Game with ID {itemDto.ProductId} not found.");
            }

            var cart = await _cartRepository.GetCartAsync(userId);
            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == itemDto.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += itemDto.Quantity;
                existingItem.Price = (decimal)game.Price;

                if (existingItem.Quantity <= 0)
                {
                    cart.Items.Remove(existingItem);
                    _logger.LogInformation("Removed game {ProductId} from cart for user {UserId} as quantity reached zero or less.",
                        itemDto.ProductId, userId);
                }
                else
                {
                    _logger.LogInformation("Updated quantity for game {ProductId} in cart for user {UserId}. New quantity: {Quantity}",
                        itemDto.ProductId, userId, existingItem.Quantity);
                }
            }
            else
            {
                if (itemDto.Quantity > 0)
                {
                    var newItem = new ShoppingCartItem
                    {
                        ProductId = itemDto.ProductId,
                        Quantity = itemDto.Quantity,
                        Price = (decimal)game.Price
                    };
                    cart.Items.Add(newItem);
                    _logger.LogInformation("Added new game {ProductId} to cart for user {UserId} with quantity {Quantity}.",
                        itemDto.ProductId, userId, itemDto.Quantity);
                }
                else
                {
                    _logger.LogWarning("Attempted to subtract quantity from non-existent item {ProductId} for user {UserId}.",
                        itemDto.ProductId, userId);
                }
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
                cart.Items.Remove(itemToRemove);
                _logger.LogInformation("Removed item {ProductId} from cart for user {UserId}.", productId, userId);
                var updatedCart = await _cartRepository.UpdateCartAsync(cart);
                return _mapper.Map<ShoppingCartDto>(updatedCart);
            }

            _logger.LogWarning("Attempted to remove non-existent item {ProductId} from cart for user {UserId}.", productId, userId);
            return _mapper.Map<ShoppingCartDto>(cart);
        }

        public async Task ClearCartAsync(Guid userId)
        {
            _logger.LogInformation("Clearing cart for user {UserId}.", userId);
            await _cartRepository.DeleteCartAsync(userId);
        }
    }
}
