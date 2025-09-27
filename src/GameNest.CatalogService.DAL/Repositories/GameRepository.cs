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
    public class GameRepository : GenericRepository<Game>, IGameRepository
    {
        public GameRepository(CatalogDbContext context)
            : base(context)
        {
        }

        public async Task<PagedList<Game>> GetGamesPagedAsync(
            GameParameters parameters,
            ISortHelper<Game>? sortHelper = null,
            CancellationToken cancellationToken = default)
        {
            var spec = new GameWithFiltersSpecification(parameters);
            var query = ApplySpecification(spec)
                .ApplySorting(parameters.OrderBy, sortHelper);

            return await query.ToPagedListAsync(parameters, cancellationToken);
        }

        public async Task<Game?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(g => g.Publisher)
                .Include(g => g.GameGenres)
                    .ThenInclude(gg => gg.Genre)
                .Include(g => g.GamePlatforms)
                    .ThenInclude(gp => gp.Platform)
                .Include(g => g.GameDeveloperRoles)
                    .ThenInclude(gdr => gdr.Developer)
                .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
        }
    }
}
