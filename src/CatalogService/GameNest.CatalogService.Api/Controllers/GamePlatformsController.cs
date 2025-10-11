using GameNest.CatalogService.BLL.DTOs.GamePlatforms;
using GameNest.CatalogService.BLL.Services.Interfaces;
using GameNest.CatalogService.Domain.Entities.Parameters;
using Microsoft.AspNetCore.Mvc;

namespace GameNest.CatalogService.Api.Controllers
{
    [ApiController]
    [Route("api/Catalog/[controller]")]
    public class GamePlatformsController : ControllerBase
    {
        private readonly IGamePlatformService _gamePlatformService;

        public GamePlatformsController(IGamePlatformService gamePlatformService)
        {
            _gamePlatformService = gamePlatformService;
        }

        /// <summary>
        /// Get all game-platform relations with pagination and filtering.
        /// </summary>
        /// <param name="parameters">Filtering and pagination parameters</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">Returns the paginated list of game-platform relations</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<GamePlatformDto>>> GetGamePlatforms([FromQuery] GamePlatformParameters parameters, CancellationToken cancellationToken)
        {
            var gamePlatforms = await _gamePlatformService.GetGamePlatformsPagedAsync(parameters, cancellationToken);
            return Ok(gamePlatforms);
        }

        /// <summary>
        /// Get a specific game-platform relation by Id.
        /// </summary>
        /// <param name="id">GamePlatform Id</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">Returns the game-platform relation details</response>
        /// <response code="404">GamePlatform not found</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GamePlatformDto>> GetGamePlatformById(Guid id, CancellationToken cancellationToken)
        {
            var gamePlatform = await _gamePlatformService.GetGamePlatformByIdAsync(id, cancellationToken);
            if (gamePlatform == null) return NotFound();
            return Ok(gamePlatform);
        }

        /// <summary>
        /// Create a new game-platform relation.
        /// </summary>
        /// <param name="dto">GamePlatform creation data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="201">GamePlatform created successfully</response>
        /// <response code="400">Validation error</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<GamePlatformDto>> CreateGamePlatform([FromBody] GamePlatformCreateDto dto, CancellationToken cancellationToken)
        {
            var createdGamePlatform = await _gamePlatformService.CreateGamePlatformAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetGamePlatformById), new { id = createdGamePlatform.Id }, createdGamePlatform);
        }

        /// <summary>
        /// Update an existing game-platform relation.
        /// </summary>
        /// <param name="id">GamePlatform Id</param>
        /// <param name="updateDto">GamePlatform update data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">GamePlatform updated successfully</response>
        /// <response code="400">Validation error or ID mismatch</response>
        /// <response code="404">GamePlatform not found</response>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GamePlatformDto>> UpdateGamePlatform(Guid id, [FromBody] GamePlatformUpdateDto updateDto, CancellationToken cancellationToken)
        {
            var updatedGamePlatform = await _gamePlatformService.UpdateGamePlatformAsync(id, updateDto, cancellationToken);
            return Ok(updatedGamePlatform);
        }

        /// <summary>
        /// Delete a game-platform relation by Id.
        /// </summary>
        /// <param name="id">GamePlatform Id</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="204">GamePlatform deleted successfully</response>
        /// <response code="404">GamePlatform not found</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteGamePlatform(Guid id, CancellationToken cancellationToken)
        {
            await _gamePlatformService.DeleteGamePlatformAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
