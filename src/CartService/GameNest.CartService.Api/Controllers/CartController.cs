using GameNest.CartService.BLL.DTOs;
using GameNest.CartService.BLL.Interfaces;
using GameNest.ServiceDefaults.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameNest.CartService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartService cartService, ILogger<CartController> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }

        /// <summary>
        /// Get a user's shopping cart.
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <response code="200">Returns the user's shopping cart</response>
        /// <response code="503">Service (Redis) is unavailable</response>
        [HttpGet]
        [ProducesResponseType(typeof(ShoppingCartDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<ShoppingCartDto>> GetCart()
        {
            var userId = User.GetUserId();
            _logger.LogInformation("Attempting to get cart for user {UserId}", userId);
            var cart = await _cartService.GetCartAsync(userId);
            return Ok(cart);
        }

        /// <summary>
        /// Add an item to a user's cart or update its quantity.
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <param name="itemDto">The item to add or update</param>
        /// <response code="200">Returns the updated shopping cart</response>
        /// <response code="400">Invalid item data (e.g., quantity is 0)</response>
        /// <response code="503">Service (Redis) is unavailable</response>
        [HttpPost("items")]
        [ProducesResponseType(typeof(ShoppingCartDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<ShoppingCartDto>> AddOrUpdateItem([FromBody] CartItemChangeDto itemDto)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var userId = User.GetUserId();
            _logger.LogInformation("Attempting to add/update item {ProductId} for user {UserId}", itemDto.ProductId, userId);
            var updatedCart = await _cartService.AddOrUpdateItemAsync(userId, itemDto);
            return Ok(updatedCart);
        }

        /// <summary>
        /// Remove an item from a user's cart.
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <param name="productId">The ID of the product to remove</param>
        /// <response code="200">Returns the updated shopping cart</response>
        /// <response code="503">Service (Redis) is unavailable</response>
        [HttpDelete("items/{productId:guid}")]
        [ProducesResponseType(typeof(ShoppingCartDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<ShoppingCartDto>> RemoveItem(Guid productId)
        {
            var userId = User.GetUserId();
            _logger.LogInformation("Attempting to remove item {ProductId} for user {UserId}", productId, userId);
            var updatedCart = await _cartService.RemoveItemAsync(userId, productId);
            return Ok(updatedCart);
        }

        /// <summary>
        /// Clear all items from a user's cart.
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <response code="204">Cart cleared successfully</response>
        /// <response code="503">Service (Redis) is unavailable</response>
        [HttpDelete("clear")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> ClearCart()
        {
            var userId = User.GetUserId();
            _logger.LogInformation("Attempting to clear cart for user {UserId}", userId);
            await _cartService.ClearCartAsync(userId);
            return NoContent();
        }
    }
}
