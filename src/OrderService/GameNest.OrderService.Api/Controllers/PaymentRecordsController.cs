using GameNest.OrderService.BLL.DTOs.PaymentRecord;
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
    public class PaymentRecordsController : ControllerBase
    {
        private readonly IPaymentRecordService _paymentRecordService;

        public PaymentRecordsController(IPaymentRecordService paymentRecordService)
        {
            _paymentRecordService = paymentRecordService;
        }

        /// <summary>
        /// Get all payment records.
        /// </summary>
        /// <response code="200">Returns the list of payment records</response>
        [HttpGet]
        [RequirePermission("payments:read")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PaymentRecordDto>>> GetAll(CancellationToken ct)
        {
            var records = await _paymentRecordService.GetAllAsync(ct);
            return Ok(records);
        }

        /// <summary>
        /// Get payment record by Id.
        /// </summary>
        /// <param name="id">Payment record Id</param>
        /// <response code="200">Returns the payment record</response>
        /// <response code="404">Payment record not found</response>
        [HttpGet("{id:guid}")]
        [RequirePermission("payments:read")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PaymentRecordDto>> GetById(Guid id, CancellationToken ct)
        {
            var paymentRecord = await _paymentRecordService.GetByIdAsync(id, ct);
            return Ok(paymentRecord);
        }

        /// <summary>
        /// Get all payment records for a specific order.
        /// </summary>
        /// <param name="orderId">Order Id</param>
        /// <response code="200">Returns list of payment records for the order</response>
        [HttpGet("order/{orderId:guid}")]
        [RequirePermission("payments:read")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PaymentRecordDto>>> GetByOrderId(Guid orderId, CancellationToken ct)
        {
            var records = await _paymentRecordService.GetByOrderIdAsync(orderId, ct);
            return Ok(records);
        }

        /// <summary>
        /// Create a new payment record.
        /// </summary>
        /// <param name="dto">Payment record creation data</param>
        /// <response code="201">Payment record created successfully</response>
        /// <response code="400">Validation error</response>
        [HttpPost]
        [RequirePermission("payments:create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PaymentRecordDto>> Create([FromBody] PaymentRecordCreateDto dto, CancellationToken ct)
        {
            var created = await _paymentRecordService.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Update an existing payment record.
        /// </summary>
        /// <param name="id">Payment record Id</param>
        /// <param name="dto">Payment record update data</param>
        /// <response code="200">Payment record updated successfully</response>
        /// <response code="400">Validation error</response>
        /// <response code="404">Payment record not found</response>
        [HttpPut("{id:guid}")]
        [RequirePermission("payments:update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PaymentRecordDto>> Update(Guid id, [FromBody] PaymentRecordUpdateDto dto, CancellationToken ct)
        {
            var updated = await _paymentRecordService.UpdateAsync(id, dto, ct);
            return Ok(updated);
        }

        /// <summary>
        /// Delete a payment record.
        /// </summary>
        /// <param name="id">Payment record Id</param>
        /// <param name="soft">Soft delete flag (default true)</param>
        /// <response code="204">Payment record deleted successfully</response>
        /// <response code="404">Payment record not found</response>
        [HttpDelete("{id:guid}")]
        [RequirePermission("payments:delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, [FromQuery] bool soft = true, CancellationToken ct = default)
        {
            await _paymentRecordService.DeleteAsync(id, soft, ct);
            return NoContent();
        }
    }
}