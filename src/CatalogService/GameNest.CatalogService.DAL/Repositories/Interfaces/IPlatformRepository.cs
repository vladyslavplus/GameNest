using GameNest.CatalogService.DAL.Helpers;
using GameNest.CatalogService.Domain.Entities;
using GameNest.CatalogService.Domain.Entities.Parameters;

namespace GameNest.CatalogService.DAL.Repositories.Interfaces
{
    public interface IPlatformRepository : IGenericRepository<Platform>
    {
        Task<PagedList<Platform>> GetPlatformsPagedAsync(PlatformParameters parameters, ISortHelper<Platform>? sortHelper = null, CancellationToken cancellationToken = default);
    }
}
