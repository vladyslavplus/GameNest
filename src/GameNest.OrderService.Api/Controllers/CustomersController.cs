using GameNest.OrderService.BLL.DTOs.Customer;
using GameNest.OrderService.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GameNest.OrderService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        /// <summary>
        /// Get all customers.
        /// </summary>
        /// <response code="200">Returns the list of customers</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAll(CancellationToken ct)
        {
            var customers = await _customerService.GetAllAsync(ct);
            return Ok(customers);
        }

        /// <summary>
        /// Get customer by Id.
        /// </summary>
        /// <param name="id">Customer Id</param>
        /// <response code="200">Returns the customer</response>
        /// <response code="404">Customer not found</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CustomerDto>> GetById(Guid id, CancellationToken ct)
        {
            var customer = await _customerService.GetByIdAsync(id, ct);
            return Ok(customer);
        }

        /// <summary>
        /// Create a new customer.
        /// </summary>
        /// <param name="dto">Customer creation data</param>
        /// <response code="201">Customer created successfully</response>
        /// <response code="400">Validation error</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CustomerDto>> Create([FromBody] CustomerCreateDto dto, CancellationToken ct)
        {
            var created = await _customerService.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Update an existing customer.
        /// </summary>
        /// <param name="id">Customer Id</param>
        /// <param name="dto">Customer update data</param>
        /// <response code="200">Customer updated successfully</response>
        /// <response code="400">Validation error</response>
        /// <response code="404">Customer not found</response>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CustomerDto>> Update(Guid id, [FromBody] CustomerUpdateDto dto, CancellationToken ct)
        {
            var updated = await _customerService.UpdateAsync(id, dto, ct);
            return Ok(updated);
        }

        /// <summary>
        /// Delete a customer.
        /// </summary>
        /// <param name="id">Customer Id</param>
        /// <param name="soft">Soft delete flag (default true)</param>
        /// <response code="204">Customer deleted successfully</response>
        /// <response code="404">Customer not found</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, [FromQuery] bool soft = true, CancellationToken ct = default)
        {
            await _customerService.DeleteAsync(id, soft, ct);
            return NoContent();
        }
    }
}