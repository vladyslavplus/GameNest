using GameNest.CatalogService.BLL.DTOs.Publishers;
using GameNest.CatalogService.BLL.Services.Interfaces;
using GameNest.CatalogService.Domain.Entities.Parameters;
using Microsoft.AspNetCore.Mvc;

namespace GameNest.CatalogService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PublishersController : ControllerBase
    {
        private readonly IPublisherService _publisherService;

        public PublishersController(IPublisherService publisherService)
        {
            _publisherService = publisherService;
        }

        /// <summary>
        /// Get all publishers with pagination and filtering.
        /// </summary>
        /// <param name="parameters">Filtering and pagination parameters</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">Returns the paginated list of publishers</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PublisherDto>>> GetPublishers([FromQuery] PublisherParameters parameters, CancellationToken cancellationToken)
        {
            var publishers = await _publisherService.GetPublishersPagedAsync(parameters, cancellationToken);
            return Ok(publishers);
        }

        /// <summary>
        /// Get a publisher by Id.
        /// </summary>
        /// <param name="id">Publisher Id</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">Returns the publisher details including associated games</response>
        /// <response code="404">Publisher not found</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PublisherDto>> GetPublisherById(Guid id, CancellationToken cancellationToken)
        {
            var publisher = await _publisherService.GetPublisherByIdAsync(id, cancellationToken);
            if (publisher == null) return NotFound();
            return Ok(publisher);
        }

        /// <summary>
        /// Create a new publisher.
        /// </summary>
        /// <param name="dto">Publisher creation data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="201">Publisher created successfully</response>
        /// <response code="400">Validation error</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PublisherDto>> CreatePublisher([FromBody] PublisherCreateDto dto, CancellationToken cancellationToken)
        {
            var createdPublisher = await _publisherService.CreatePublisherAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetPublisherById), new { id = createdPublisher.Id }, createdPublisher);
        }

        /// <summary>
        /// Update an existing publisher.
        /// </summary>
        /// <param name="id">Publisher Id</param>
        /// <param name="updateDto">Publisher update data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">Publisher updated successfully</response>
        /// <response code="400">Validation error or ID mismatch</response>
        /// <response code="404">Publisher not found</response>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PublisherDto>> UpdatePublisher(Guid id, [FromBody] PublisherUpdateDto updateDto, CancellationToken cancellationToken)
        {
            var updatedPublisher = await _publisherService.UpdatePublisherAsync(id, updateDto, cancellationToken);
            return Ok(updatedPublisher);
        }

        /// <summary>
        /// Delete a publisher by Id.
        /// </summary>
        /// <param name="id">Publisher Id</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="204">Publisher deleted successfully</response>
        /// <response code="404">Publisher not found</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePublisher(Guid id, CancellationToken cancellationToken)
        {
            await _publisherService.DeletePublisherAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
