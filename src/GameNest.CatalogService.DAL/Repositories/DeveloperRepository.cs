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
    public class DeveloperRepository : GenericRepository<Developer>, IDeveloperRepository
    {
        public DeveloperRepository(CatalogDbContext context)
            : base(context)
        {
        }

        public async Task<PagedList<Developer>> GetDevelopersPagedAsync(
            DeveloperParameters parameters,
            ISortHelper<Developer>? sortHelper = null,
            CancellationToken cancellationToken = default)
        {
            var spec = new DeveloperWithFiltersSpecification(parameters);
            var query = ApplySpecification(spec)
                            .ApplySorting(parameters.OrderBy, sortHelper);

            return await query.ToPagedListAsync(parameters, cancellationToken);
        }

        public async Task<Developer?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var developer = await _dbSet.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
            if (developer == null) return null;

            await _context.Entry(developer)
                .Collection(d => d.GameDeveloperRoles)
                .LoadAsync(cancellationToken);

            foreach (var gdr in developer.GameDeveloperRoles)
            {
                await _context.Entry(gdr)
                    .Reference(x => x.Game)
                    .LoadAsync(cancellationToken);

                await _context.Entry(gdr)
                    .Reference(x => x.Role)
                    .LoadAsync(cancellationToken);
            }

            return developer;
        }
    }
}