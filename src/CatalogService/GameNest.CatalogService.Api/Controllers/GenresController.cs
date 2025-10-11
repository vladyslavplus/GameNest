using GameNest.CatalogService.BLL.DTOs.Genres;
using GameNest.CatalogService.BLL.Services.Interfaces;
using GameNest.CatalogService.Domain.Entities.Parameters;
using Microsoft.AspNetCore.Mvc;

namespace GameNest.CatalogService.Api.Controllers
{
    [ApiController]
    [Route("api/Catalog/[controller]")]
    public class GenresController : ControllerBase
    {
        private readonly IGenreService _genreService;

        public GenresController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        /// <summary>
        /// Get all genres with pagination and filtering.
        /// </summary>
        /// <param name="parameters">Filtering and pagination parameters</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">Returns the paginated list of genres</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<GenreDto>>> GetGenres([FromQuery] GenreParameters parameters, CancellationToken cancellationToken)
        {
            var genres = await _genreService.GetGenresPagedAsync(parameters, cancellationToken);
            return Ok(genres);
        }

        /// <summary>
        /// Get a genre by Id.
        /// </summary>
        /// <param name="id">Genre Id</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">Returns the genre details</response>
        /// <response code="404">Genre not found</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GenreDto>> GetGenreById(Guid id, CancellationToken cancellationToken)
        {
            var genre = await _genreService.GetGenreByIdAsync(id, cancellationToken);
            if (genre == null) return NotFound();
            return Ok(genre);
        }

        /// <summary>
        /// Create a new genre.
        /// </summary>
        /// <param name="dto">Genre creation data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="201">Genre created successfully</response>
        /// <response code="400">Validation error</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<GenreDto>> CreateGenre([FromBody] GenreCreateDto dto, CancellationToken cancellationToken)
        {
            var createdGenre = await _genreService.CreateGenreAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetGenreById), new { id = createdGenre.Id }, createdGenre);
        }

        /// <summary>
        /// Update an existing genre.
        /// </summary>
        /// <param name="id">Genre Id</param>
        /// <param name="updateDto">Genre update data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">Genre updated successfully</response>
        /// <response code="400">Validation error or ID mismatch</response>
        /// <response code="404">Genre not found</response>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GenreDto>> UpdateGenre(Guid id, [FromBody] GenreUpdateDto updateDto, CancellationToken cancellationToken)
        {
            var updatedGenre = await _genreService.UpdateGenreAsync(id, updateDto, cancellationToken);
            return Ok(updatedGenre);
        }

        /// <summary>
        /// Delete a genre by Id.
        /// </summary>
        /// <param name="id">Genre Id</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="204">Genre deleted successfully</response>
        /// <response code="404">Genre not found</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteGenre(Guid id, CancellationToken cancellationToken)
        {
            await _genreService.DeleteGenreAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
