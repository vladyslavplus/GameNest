using GameNest.CatalogService.BLL.DTOs.GameGenres;
using GameNest.CatalogService.BLL.Services.Interfaces;
using GameNest.CatalogService.Domain.Entities.Parameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameNest.CatalogService.Api.Controllers
{
    [ApiController]
    [Route("api/Catalog/[controller]")]
    public class GameGenresController : ControllerBase
    {
        private readonly IGameGenreService _gameGenreService;

        public GameGenresController(IGameGenreService gameGenreService)
        {
            _gameGenreService = gameGenreService;
        }

        /// <summary>
        /// Get all game genres with pagination and filtering.
        /// </summary>
        /// <param name="parameters">Filtering and pagination parameters</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">Returns the paginated list of game genres</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<GameGenreDto>>> GetGameGenres(
            [FromQuery] GameGenreParameters parameters,
            CancellationToken cancellationToken)
        {
            var gameGenres = await _gameGenreService.GetGameGenresPagedAsync(parameters, cancellationToken);
            return Ok(gameGenres);
        }

        /// <summary>
        /// Get a game genre by Id.
        /// </summary>
        /// <param name="id">Game genre Id</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">Returns the game genre details</response>
        /// <response code="404">Game genre not found</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GameGenreDto>> GetGameGenreById(Guid id, CancellationToken cancellationToken)
        {
            var gameGenre = await _gameGenreService.GetGameGenreByIdAsync(id, cancellationToken);
            if (gameGenre == null) return NotFound();
            return Ok(gameGenre);
        }

        /// <summary>
        /// Create a new game genre (relation between Game and Genre).
        /// </summary>
        /// <param name="dto">GameGenre creation data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="201">GameGenre created successfully</response>
        /// <response code="400">Validation error</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<GameGenreDto>> CreateGameGenre([FromBody] GameGenreCreateDto dto, CancellationToken cancellationToken)
        {
            var createdGameGenre = await _gameGenreService.CreateGameGenreAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetGameGenreById), new { id = createdGameGenre.Id }, createdGameGenre);
        }

        /// <summary>
        /// Delete a game genre by Id.
        /// </summary>
        /// <param name="id">Game genre Id</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="204">GameGenre deleted successfully</response>
        /// <response code="404">GameGenre not found</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteGameGenre(Guid id, CancellationToken cancellationToken)
        {
            await _gameGenreService.DeleteGameGenreAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
