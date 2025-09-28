using GameNest.OrderService.BLL.DTOs.Order;
using GameNest.OrderService.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GameNest.OrderService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Get all orders.
        /// </summary>
        /// <response code="200">Returns the list of orders</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAll(CancellationToken ct)
        {
            var orders = await _orderService.GetAllAsync(ct);
            return Ok(orders);
        }

        /// <summary>
        /// Get order by Id.
        /// </summary>
        /// <param name="id">Order Id</param>
        /// <response code="200">Returns the order</response>
        /// <response code="404">Order not found</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderDto>> GetById(Guid id, CancellationToken ct)
        {
            var order = await _orderService.GetByIdAsync(id, ct);
            return Ok(order);
        }

        /// <summary>
        /// Create a new order.
        /// </summary>
        /// <param name="dto">Order creation data</param>
        /// <response code="201">Order created successfully</response>
        /// <response code="400">Validation error</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<OrderDto>> Create([FromBody] OrderCreateDto dto, CancellationToken ct)
        {
            var created = await _orderService.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Update an existing order status.
        /// </summary>
        /// <param name="id">Order Id</param>
        /// <param name="dto">Order update data (status only)</param>
        /// <response code="200">Order updated successfully</response>
        /// <response code="400">Validation error</response>
        /// <response code="404">Order not found</response>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderDto>> Update(Guid id, [FromBody] OrderUpdateDto dto, CancellationToken ct)
        {
            var updated = await _orderService.UpdateAsync(id, dto, ct);
            return Ok(updated);
        }

        /// <summary>
        /// Delete an order and its items.
        /// </summary>
        /// <param name="id">Order Id</param>
        /// <param name="soft">Soft delete flag (default true)</param>
        /// <response code="204">Order deleted successfully</response>
        /// <response code="404">Order not found</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, [FromQuery] bool soft = true, CancellationToken ct = default)
        {
            await _orderService.DeleteAsync(id, soft, ct);
            return NoContent();
        }
    }
}