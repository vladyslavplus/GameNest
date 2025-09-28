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
    public class GamePlatformRepository : GenericRepository<GamePlatform>, IGamePlatformRepository
    {
        public GamePlatformRepository(CatalogDbContext context)
            : base(context)
        {
        }

        public async Task<PagedList<GamePlatform>> GetGamePlatformsPagedAsync(
            GamePlatformParameters parameters,
            ISortHelper<GamePlatform>? sortHelper = null,
            CancellationToken cancellationToken = default)
        {
            var spec = new GamePlatformWithFiltersSpecification(parameters);
            var query = ApplySpecification(spec)
                .ApplySorting(parameters.OrderBy, sortHelper);

            return await query.ToPagedListAsync(parameters, cancellationToken);
        }

        public async Task<GamePlatform?> GetByIdWithReferencesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var gp = await _dbSet.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (gp == null) return null;

            await _context.Entry(gp).Reference(x => x.Game).LoadAsync(cancellationToken);
            await _context.Entry(gp).Reference(x => x.Platform).LoadAsync(cancellationToken);

            return gp;
        }
    }
}
