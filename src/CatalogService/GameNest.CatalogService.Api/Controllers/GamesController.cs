using GameNest.CatalogService.BLL.DTOs.Games;
using GameNest.CatalogService.BLL.Services.Interfaces;
using GameNest.CatalogService.Domain.Entities.Parameters;
using Microsoft.AspNetCore.Mvc;

namespace GameNest.CatalogService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GamesController : ControllerBase
    {
        private readonly IGameService _gameService;

        public GamesController(IGameService gameService)
        {
            _gameService = gameService;
        }

        /// <summary>
        /// Get all games with pagination and filtering.
        /// </summary>
        /// <param name="parameters">Filtering and pagination parameters</param>
        /// <response code="200">Returns the list of games</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<GameDto>>> GetGames([FromQuery] GameParameters parameters, CancellationToken cancellationToken)
        {
            var games = await _gameService.GetGamesPagedAsync(parameters, cancellationToken);
            return Ok(games);
        }

        /// <summary>
        /// Get a game by Id.
        /// </summary>
        /// <param name="id">Game Id</param>
        /// <response code="200">Returns the game</response>
        /// <response code="404">Game not found</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GameDto>> GetGameById(Guid id, CancellationToken cancellationToken)
        {
            var game = await _gameService.GetGameByIdAsync(id, cancellationToken);
            if (game == null) return NotFound();
            return Ok(game);
        }

        /// <summary>
        /// Create a new game.
        /// </summary>
        /// <param name="gameCreateDto">Game creation data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="201">Game created successfully</response>
        /// <response code="400">Validation error</response>
        /// <response code="409">Conflict – game already exists</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<GameDto>> CreateGame([FromBody] GameCreateDto dto, CancellationToken cancellationToken)
        {
            var createdGame = await _gameService.CreateGameAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetGameById), new { id = createdGame.Id }, createdGame);
        }

        /// <summary>
        /// Update an existing game.
        /// </summary>
        /// <param name="id">Game Id</param>
        /// <param name="gameUpdateDto">Game update data</param>
        /// <response code="200">Game updated successfully</response>
        /// <response code="400">ID mismatch or validation error</response>
        /// <response code="404">Game not found</response>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GameDto>> UpdateGame(Guid id, [FromBody] GameUpdateDto updateDto, CancellationToken cancellationToken)
        {
            var updatedGame = await _gameService.UpdateGameAsync(id, updateDto, cancellationToken);
            return Ok(updatedGame);
        }

        /// <summary>
        /// Delete a game by Id.
        /// </summary>
        /// <param name="id">Game Id</param>
        /// <response code="204">Game deleted successfully</response>
        /// <response code="404">Game not found</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteGame(Guid id, CancellationToken cancellationToken)
        {
            await _gameService.DeleteGameAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
