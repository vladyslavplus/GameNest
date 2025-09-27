using GameNest.CatalogService.DAL.Helpers;
using GameNest.CatalogService.Domain.Entities;
using GameNest.CatalogService.Domain.Entities.Parameters;

namespace GameNest.CatalogService.DAL.Repositories.Interfaces
{
    public interface IGamePlatformRepository : IGenericRepository<GamePlatform>
    {
        Task<PagedList<GamePlatform>> GetGamePlatformsPagedAsync(GamePlatformParameters parameters, ISortHelper<GamePlatform>? sortHelper = null, CancellationToken cancellationToken = default);
        Task<GamePlatform?> GetByIdWithReferencesAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
