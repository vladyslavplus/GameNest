using GameNest.CatalogService.DAL.Data;
using GameNest.CatalogService.DAL.Extensions;
using GameNest.CatalogService.DAL.Helpers;
using GameNest.CatalogService.DAL.Repositories.Interfaces;
using GameNest.CatalogService.DAL.Specifications;
using GameNest.CatalogService.Domain.Entities;
using GameNest.CatalogService.Domain.Entities.Parameters;

namespace GameNest.CatalogService.DAL.Repositories
{
    public class GenreRepository : GenericRepository<Genre>, IGenreRepository
    {
        public GenreRepository(CatalogDbContext context)
            : base(context)
        {
        }

        public async Task<PagedList<Genre>> GetGenresPagedAsync(
            GenreParameters parameters,
            ISortHelper<Genre>? sortHelper = null,
            CancellationToken cancellationToken = default)
        {
            var spec = new GenreWithFiltersSpecification(parameters);
            var query = ApplySpecification(spec)
                .ApplySorting(parameters.OrderBy, sortHelper);

            return await query.ToPagedListAsync(parameters, cancellationToken);
        }
    }
}
