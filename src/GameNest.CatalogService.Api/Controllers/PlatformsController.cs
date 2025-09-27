using GameNest.CatalogService.BLL.DTOs.Platforms;
using GameNest.CatalogService.BLL.Services.Interfaces;
using GameNest.CatalogService.Domain.Entities.Parameters;
using Microsoft.AspNetCore.Mvc;

namespace GameNest.CatalogService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformService _platformService;

        public PlatformsController(IPlatformService platformService)
        {
            _platformService = platformService;
        }

        /// <summary>
        /// Get all platforms with pagination and filtering.
        /// </summary>
        /// <param name="parameters">Filtering and pagination parameters</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">Returns the paginated list of platforms</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PlatformDto>>> GetPlatforms([FromQuery] PlatformParameters parameters, CancellationToken cancellationToken)
        {
            var platforms = await _platformService.GetPlatformsPagedAsync(parameters, cancellationToken);
            return Ok(platforms);
        }

        /// <summary>
        /// Get a platform by Id.
        /// </summary>
        /// <param name="id">Platform Id</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">Returns the platform details</response>
        /// <response code="404">Platform not found</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PlatformDto>> GetPlatformById(Guid id, CancellationToken cancellationToken)
        {
            var platform = await _platformService.GetPlatformByIdAsync(id, cancellationToken);
            if (platform == null) return NotFound();
            return Ok(platform);
        }

        /// <summary>
        /// Create a new platform.
        /// </summary>
        /// <param name="dto">Platform creation data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="201">Platform created successfully</response>
        /// <response code="400">Validation error</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PlatformDto>> CreatePlatform([FromBody] PlatformCreateDto dto, CancellationToken cancellationToken)
        {
            var createdPlatform = await _platformService.CreatePlatformAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetPlatformById), new { id = createdPlatform.Id }, createdPlatform);
        }

        /// <summary>
        /// Update an existing platform.
        /// </summary>
        /// <param name="id">Platform Id</param>
        /// <param name="updateDto">Platform update data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">Platform updated successfully</response>
        /// <response code="400">Validation error or ID mismatch</response>
        /// <response code="404">Platform not found</response>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PlatformDto>> UpdatePlatform(Guid id, [FromBody] PlatformUpdateDto updateDto, CancellationToken cancellationToken)
        {
            var updatedPlatform = await _platformService.UpdatePlatformAsync(id, updateDto, cancellationToken);
            return Ok(updatedPlatform);
        }

        /// <summary>
        /// Delete a platform by Id.
        /// </summary>
        /// <param name="id">Platform Id</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="204">Platform deleted successfully</response>
        /// <response code="404">Platform not found</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePlatform(Guid id, CancellationToken cancellationToken)
        {
            await _platformService.DeletePlatformAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
