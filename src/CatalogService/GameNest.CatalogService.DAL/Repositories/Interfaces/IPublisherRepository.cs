using GameNest.CatalogService.DAL.Helpers;
using GameNest.CatalogService.Domain.Entities;
using GameNest.CatalogService.Domain.Entities.Parameters;

namespace GameNest.CatalogService.DAL.Repositories.Interfaces
{
    public interface IPublisherRepository : IGenericRepository<Publisher>
    {
        Task<PagedList<Publisher>> GetPublishersPagedAsync(PublisherParameters parameters, ISortHelper<Publisher>? sortHelper = null, CancellationToken cancellationToken = default);
        Task<Publisher?> GetByIdWithGamesAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
