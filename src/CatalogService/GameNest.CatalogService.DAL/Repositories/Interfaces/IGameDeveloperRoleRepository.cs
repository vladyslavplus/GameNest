using GameNest.CatalogService.DAL.Helpers;
using GameNest.CatalogService.Domain.Entities;
using GameNest.CatalogService.Domain.Entities.Parameters;

namespace GameNest.CatalogService.DAL.Repositories.Interfaces
{
    public interface IGameDeveloperRoleRepository : IGenericRepository<GameDeveloperRole>
    {
        Task<PagedList<GameDeveloperRole>> GetRolesPagedAsync(GameDeveloperRoleParameters parameters, ISortHelper<GameDeveloperRole>? sortHelper = null, CancellationToken cancellationToken = default);
        Task<GameDeveloperRole?> GetByIdWithReferencesAsync(Guid id, CancellationToken cancellationToken = default);
    }
}