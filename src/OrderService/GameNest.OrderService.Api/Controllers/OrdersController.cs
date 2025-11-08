using GameNest.OrderService.BLL.DTOs.Order;
using GameNest.OrderService.BLL.Services.Interfaces;
using GameNest.ServiceDefaults.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameNest.OrderService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        /// <summary>
        /// Get all orders.
        /// </summary>
        /// <response code="200">Returns the list of orders</response>
        /// <response code="401">User is not authorized</response>
        [HttpGet]
        [RequirePermission("orders:read")]
        [ProducesResponseType(typeof(IEnumerable<OrderDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAll(CancellationToken ct)
        {
            _logger.LogInformation("Attempting to retrieve all orders.");
            var orders = await _orderService.GetAllAsync(ct);
            return Ok(orders);
        }

        /// <summary>
        /// Get order by Id.
        /// </summary>
        /// <param name="id">Order Id</param>
        /// <response code="200">Returns the order</response>
        /// <response code="401">User is not authorized</response>
        /// <response code="404">Order not found</response>
        [HttpGet("{id:guid}")]
        [RequirePermission("orders:read")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderDto>> GetById(Guid id, CancellationToken ct)
        {
            _logger.LogInformation("Attempting to retrieve order {OrderId}", id);
            var order = await _orderService.GetByIdAsync(id, ct);
            return Ok(order);
        }

        /// <summary>
        /// Create a new order from the user's current cart.
        /// </summary>
        /// <param name="dto">Order creation data (e.g., address info)</param>
        /// <response code="201">Order created successfully</response>
        /// <response code="400">Validation error (e.g., empty cart)</response>
        /// <response code="401">User is not authorized</response>
        [HttpPost]
        [RequirePermission("orders:create")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<OrderDto>> Create([FromBody] OrderCreateDto dto, CancellationToken ct)
        {
            var userId = User.GetUserId();
            _logger.LogInformation("User {UserId} attempting to create order.", userId);
            var created = await _orderService.CreateAsync(userId, dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Update an existing order status.
        /// </summary>
        /// <param name="id">Order Id</param>
        /// <param name="dto">Order update data (status only)</param>
        /// <response code="200">Order updated successfully</response>
        /// <response code="400">Validation error</response>
        /// <response code="401">User is not authorized</response>
        /// <response code="404">Order not found</response>
        [HttpPut("{id:guid}")]
        [RequirePermission("orders:update")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderDto>> Update(Guid id, [FromBody] OrderUpdateDto dto, CancellationToken ct)
        {
            _logger.LogInformation("Attempting to update status for order {OrderId}", id);
            var updated = await _orderService.UpdateAsync(id, dto, ct);
            return Ok(updated);
        }

        /// <summary>
        /// Delete an order and its items.
        /// </summary>
        /// <param name="id">Order Id</param>
        /// <param name="soft">Soft delete flag (default true)</param>
        /// <response code="204">Order deleted successfully</response>
        /// <response code="401">User is not authorized</response>
        /// <response code="404">Order not found</response>
        [HttpDelete("{id:guid}")]
        [RequirePermission("orders:delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, [FromQuery] bool soft = true, CancellationToken ct = default)
        {
            _logger.LogInformation("Attempting to delete order {OrderId} (Soft={SoftDelete})", id, soft);
            await _orderService.DeleteAsync(id, soft, ct);
            return NoContent();
        }
    }
}