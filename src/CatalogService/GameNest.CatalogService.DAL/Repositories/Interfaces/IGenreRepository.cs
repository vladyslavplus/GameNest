using GameNest.CatalogService.DAL.Helpers;
using GameNest.CatalogService.Domain.Entities;
using GameNest.CatalogService.Domain.Entities.Parameters;

namespace GameNest.CatalogService.DAL.Repositories.Interfaces
{
    public interface IGenreRepository : IGenericRepository<Genre>
    {
        Task<PagedList<Genre>> GetGenresPagedAsync(GenreParameters parameters, ISortHelper<Genre>? sortHelper = null, CancellationToken cancellationToken = default);
    }
}
