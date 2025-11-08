using GameNest.OrderService.BLL.DTOs.OrderItem;
using GameNest.OrderService.BLL.Services.Interfaces;
using GameNest.ServiceDefaults.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameNest.OrderService.Api.Controllers
{
    [ApiController]
    [Route("api/Orders/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class OrderItemsController : ControllerBase
    {
        private readonly IOrderItemService _orderItemService;

        public OrderItemsController(IOrderItemService orderItemService)
        {
            _orderItemService = orderItemService;
        }

        /// <summary>
        /// Get all order items.
        /// </summary>
        /// <response code="200">Returns the list of order items</response>
        [HttpGet]
        [RequirePermission("orders:read")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OrderItemDto>>> GetAll(CancellationToken ct)
        {
            var items = await _orderItemService.GetAllAsync(ct);
            return Ok(items);
        }

        /// <summary>
        /// Get order item by Id.
        /// </summary>
        /// <param name="id">Order item Id</param>
        /// <response code="200">Returns the order item</response>
        /// <response code="404">Order item not found</response>
        [HttpGet("{id:guid}")]
        [RequirePermission("orders:read")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderItemDto>> GetById(Guid id, CancellationToken ct)
        {
            var item = await _orderItemService.GetByIdAsync(id, ct);
            return Ok(item);
        }

        /// <summary>
        /// Get all order items for a specific order.
        /// </summary>
        /// <param name="orderId">Order Id</param>
        /// <response code="200">Returns the list of order items for the order</response>
        [HttpGet("by-order/{orderId:guid}")]
        [RequirePermission("orders:read")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OrderItemDto>>> GetByOrderId(Guid orderId, CancellationToken ct)
        {
            var items = await _orderItemService.GetByOrderIdAsync(orderId, ct);
            return Ok(items);
        }
    }
}
