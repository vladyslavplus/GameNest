using GameNest.CatalogService.DAL.Helpers;
using GameNest.CatalogService.Domain.Entities;
using GameNest.CatalogService.Domain.Entities.Parameters;

namespace GameNest.CatalogService.DAL.Repositories.Interfaces
{
    public interface IDeveloperRepository : IGenericRepository<Developer>
    {
        Task<PagedList<Developer>> GetDevelopersPagedAsync(DeveloperParameters parameters, ISortHelper<Developer>? sortHelper = null, CancellationToken cancellationToken = default);
        Task<Developer?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
