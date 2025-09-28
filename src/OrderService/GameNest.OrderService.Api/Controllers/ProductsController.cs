using GameNest.OrderService.BLL.DTOs.Product;
using GameNest.OrderService.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GameNest.OrderService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Get all products.
        /// </summary>
        /// <response code="200">Returns the list of products</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll(CancellationToken ct)
        {
            var products = await _productService.GetAllAsync(ct);
            return Ok(products);
        }

        /// <summary>
        /// Get product by Id.
        /// </summary>
        /// <param name="id">Product Id</param>
        /// <response code="200">Returns the product</response>
        /// <response code="404">Product not found</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDto>> GetById(Guid id, CancellationToken ct)
        {
            var product = await _productService.GetByIdAsync(id, ct);
            return Ok(product);
        }

        /// <summary>
        /// Create a new product.
        /// </summary>
        /// <param name="dto">Product creation data</param>
        /// <response code="201">Product created successfully</response>
        /// <response code="400">Validation error</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductDto>> Create([FromBody] ProductCreateDto dto, CancellationToken ct)
        {
            var created = await _productService.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Update an existing product.
        /// </summary>
        /// <param name="id">Product Id</param>
        /// <param name="dto">Product update data</param>
        /// <response code="200">Product updated successfully</response>
        /// <response code="400">Validation error</response>
        /// <response code="404">Product not found</response>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDto>> Update(Guid id, [FromBody] ProductUpdateDto dto, CancellationToken ct)
        {
            var updated = await _productService.UpdateAsync(id, dto, ct);
            return Ok(updated);
        }

        /// <summary>
        /// Delete a product.
        /// </summary>
        /// <param name="id">Product Id</param>
        /// <param name="soft">Soft delete flag (default true)</param>
        /// <response code="204">Product deleted successfully</response>
        /// <response code="404">Product not found</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, [FromQuery] bool soft = true, CancellationToken ct = default)
        {
            await _productService.DeleteAsync(id, soft, ct);
            return NoContent();
        }
    }
}