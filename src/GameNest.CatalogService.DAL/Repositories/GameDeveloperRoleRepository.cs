using GameNest.CatalogService.DAL.Data;
using GameNest.CatalogService.DAL.Extensions;
using GameNest.CatalogService.DAL.Helpers;
using GameNest.CatalogService.DAL.Repositories.Interfaces;
using GameNest.CatalogService.DAL.Specifications;
using GameNest.CatalogService.Domain.Entities;
using GameNest.CatalogService.Domain.Entities.Parameters;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace GameNest.CatalogService.DAL.Repositories
{
    public class GameDeveloperRoleRepository : GenericRepository<GameDeveloperRole>, IGameDeveloperRoleRepository
    {
        public GameDeveloperRoleRepository(CatalogDbContext context)
            : base(context)
        {
        }

        public async Task<PagedList<GameDeveloperRole>> GetRolesPagedAsync(
            GameDeveloperRoleParameters parameters,
            ISortHelper<GameDeveloperRole>? sortHelper = null,
            CancellationToken cancellationToken = default)
        {
            var spec = new GameDeveloperRoleWithFiltersSpecification(parameters);
            var query = ApplySpecification(spec)
                .ApplySorting(parameters.OrderBy, sortHelper);

            return await query.ToPagedListAsync(parameters, cancellationToken);
        }

        public async Task<GameDeveloperRole?> GetByIdWithReferencesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var gdr = await _dbSet.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (gdr == null) return null;

            await _context.Entry(gdr).Reference(x => x.Game).LoadAsync(cancellationToken);
            await _context.Entry(gdr).Reference(x => x.Developer).LoadAsync(cancellationToken);
            await _context.Entry(gdr).Reference(x => x.Role).LoadAsync(cancellationToken);

            return gdr;
        }
    }
}
