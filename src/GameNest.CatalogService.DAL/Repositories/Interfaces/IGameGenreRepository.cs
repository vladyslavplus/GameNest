using GameNest.CatalogService.DAL.Helpers;
using GameNest.CatalogService.Domain.Entities;
using GameNest.CatalogService.Domain.Entities.Parameters;

namespace GameNest.CatalogService.DAL.Repositories.Interfaces
{
    public interface IGameGenreRepository : IGenericRepository<GameGenre>
    {
        Task<PagedList<GameGenre>> GetGameGenresPagedAsync(GameGenreParameters parameters, ISortHelper<GameGenre>? sortHelper = null, CancellationToken cancellationToken = default);
        Task<GameGenre?> GetByIdWithReferencesAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
