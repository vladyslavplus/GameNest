using GameNest.CatalogService.BLL.DTOs.Developers;
using GameNest.CatalogService.BLL.Services.Interfaces;
using GameNest.CatalogService.Domain.Entities.Parameters;
using Microsoft.AspNetCore.Mvc;

namespace GameNest.CatalogService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DevelopersController : ControllerBase
    {
        private readonly IDeveloperService _developerService;

        public DevelopersController(IDeveloperService developerService)
        {
            _developerService = developerService;
        }

        /// <summary>
        /// Get all developers with pagination and filtering.
        /// </summary>
        /// <param name="parameters">Filtering and pagination parameters</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">Returns the paginated list of developers</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DeveloperDto>>> GetDevelopers([FromQuery] DeveloperParameters parameters, CancellationToken cancellationToken)
        {
            var developers = await _developerService.GetDevelopersPagedAsync(parameters, cancellationToken);
            return Ok(developers);
        }

        /// <summary>
        /// Get a developer by Id.
        /// </summary>
        /// <param name="id">Developer Id</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">Returns the developer details including associated games and roles</response>
        /// <response code="404">Developer not found</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DeveloperDto>> GetDeveloperById(Guid id, CancellationToken cancellationToken)
        {
            var developer = await _developerService.GetDeveloperByIdAsync(id, cancellationToken);
            if (developer == null) return NotFound();
            return Ok(developer);
        }

        /// <summary>
        /// Create a new developer.
        /// </summary>
        /// <param name="dto">Developer creation data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="201">Developer created successfully</response>
        /// <response code="400">Validation error</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DeveloperDto>> CreateDeveloper([FromBody] DeveloperCreateDto dto, CancellationToken cancellationToken)
        {
            var createdDeveloper = await _developerService.CreateDeveloperAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetDeveloperById), new { id = createdDeveloper.Id }, createdDeveloper);
        }

        /// <summary>
        /// Update an existing developer.
        /// </summary>
        /// <param name="id">Developer Id</param>
        /// <param name="updateDto">Developer update data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">Developer updated successfully</response>
        /// <response code="400">Validation error or ID mismatch</response>
        /// <response code="404">Developer not found</response>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DeveloperDto>> UpdateDeveloper(Guid id, [FromBody] DeveloperUpdateDto updateDto, CancellationToken cancellationToken)
        {
            var updatedDeveloper = await _developerService.UpdateDeveloperAsync(id, updateDto, cancellationToken);
            return Ok(updatedDeveloper);
        }

        /// <summary>
        /// Delete a developer by Id.
        /// </summary>
        /// <param name="id">Developer Id</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="204">Developer deleted successfully</response>
        /// <response code="404">Developer not found</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteDeveloper(Guid id, CancellationToken cancellationToken)
        {
            await _developerService.DeleteDeveloperAsync(id, cancellationToken);
            return NoContent();
        }
    }
}