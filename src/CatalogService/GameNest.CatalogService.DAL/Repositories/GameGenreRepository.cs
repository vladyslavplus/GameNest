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
    public class GameGenreRepository : GenericRepository<GameGenre>, IGameGenreRepository
    {
        public GameGenreRepository(CatalogDbContext context)
            : base(context)
        {
        }

        public async Task<PagedList<GameGenre>> GetGameGenresPagedAsync(
            GameGenreParameters parameters,
            ISortHelper<GameGenre>? sortHelper = null,
            CancellationToken cancellationToken = default)
        {
            var spec = new GameGenreWithFiltersSpecification(parameters);
            var query = ApplySpecification(spec)
                .ApplySorting(parameters.OrderBy, sortHelper);

            return await query.ToPagedListAsync(parameters, cancellationToken);
        }

        public async Task<GameGenre?> GetByIdWithReferencesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var gameGenre = await _dbSet.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (gameGenre == null) return null;

            await _context.Entry(gameGenre).Reference(x => x.Game).LoadAsync(cancellationToken);
            await _context.Entry(gameGenre).Reference(x => x.Genre).LoadAsync(cancellationToken);

            return gameGenre;
        }
    }
}
