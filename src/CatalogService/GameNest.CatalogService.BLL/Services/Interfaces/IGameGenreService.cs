using GameNest.CatalogService.BLL.DTOs.GameGenres;
using GameNest.CatalogService.DAL.Helpers;
using GameNest.CatalogService.Domain.Entities.Parameters;

namespace GameNest.CatalogService.BLL.Services.Interfaces
{
    public interface IGameGenreService
    {
        Task<PagedList<GameGenreDto>> GetGameGenresPagedAsync(GameGenreParameters parameters, CancellationToken cancellationToken = default);
        Task<GameGenreDto?> GetGameGenreByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<GameGenreDto> CreateGameGenreAsync(GameGenreCreateDto createDto, CancellationToken cancellationToken = default);
        Task DeleteGameGenreAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
