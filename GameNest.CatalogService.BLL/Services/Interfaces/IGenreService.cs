using GameNest.CatalogService.BLL.DTOs.Genres;
using GameNest.CatalogService.DAL.Helpers;
using GameNest.CatalogService.Domain.Entities.Parameters;

namespace GameNest.CatalogService.BLL.Services.Interfaces
{
    public interface IGenreService
    {
        Task<PagedList<GenreDto>> GetGenresPagedAsync(GenreParameters parameters, CancellationToken cancellationToken = default);
        Task<GenreDto?> GetGenreByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<GenreDto> CreateGenreAsync(GenreCreateDto genreCreateDto, CancellationToken cancellationToken = default);
        Task<GenreDto> UpdateGenreAsync(Guid id, GenreUpdateDto genreUpdateDto, CancellationToken cancellationToken = default);
        Task DeleteGenreAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
