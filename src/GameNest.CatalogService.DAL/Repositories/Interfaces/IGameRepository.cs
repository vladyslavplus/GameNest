using GameNest.CatalogService.DAL.Helpers;
using GameNest.CatalogService.Domain.Entities;
using GameNest.CatalogService.Domain.Entities.Parameters;

namespace GameNest.CatalogService.DAL.Repositories.Interfaces
{
    public interface IGameRepository : IGenericRepository<Game>
    {
        Task<PagedList<Game>> GetGamesPagedAsync(GameParameters parameters, ISortHelper<Game>? sortHelper = null, CancellationToken cancellationToken = default);
        Task<Game?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
