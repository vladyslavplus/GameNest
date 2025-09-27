using GameNest.CatalogService.DAL.Data;
using GameNest.CatalogService.DAL.Extensions;
using GameNest.CatalogService.DAL.Helpers;
using GameNest.CatalogService.DAL.Repositories.Interfaces;
using GameNest.CatalogService.DAL.Specifications;
using GameNest.CatalogService.Domain.Entities;
using GameNest.CatalogService.Domain.Entities.Parameters;
using Microsoft.EntityFrameworkCore;

namespace GameNest.CatalogService.DAL.Repositories
{
    public class PublisherRepository : GenericRepository<Publisher>, IPublisherRepository
    {
        public PublisherRepository(CatalogDbContext context)
            : base(context)
        {
        }

        public async Task<PagedList<Publisher>> GetPublishersPagedAsync(
            PublisherParameters parameters,
            ISortHelper<Publisher>? sortHelper = null,
            CancellationToken cancellationToken = default)
        {
            var spec = new PublisherWithFiltersSpecification(parameters);
            var query = ApplySpecification(spec)
                .ApplySorting(parameters.OrderBy, sortHelper);

            return await query.ToPagedListAsync(parameters, cancellationToken);
        }

        public async Task<Publisher?> GetByIdWithGamesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var publisher = await _dbSet.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
            if (publisher == null) return null;

            await _context.Entry(publisher)
                .Collection(p => p.Games)
                .LoadAsync(cancellationToken);

            return publisher;
        }
    }
}
