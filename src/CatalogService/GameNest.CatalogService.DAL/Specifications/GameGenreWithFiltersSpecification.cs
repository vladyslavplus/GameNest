using Ardalis.Specification;
using GameNest.CatalogService.Domain.Entities;
using GameNest.CatalogService.Domain.Entities.Parameters;

namespace GameNest.CatalogService.DAL.Specifications
{
    public class GameGenreWithFiltersSpecification : Specification<GameGenre>
    {
        public GameGenreWithFiltersSpecification(GameGenreParameters parameters)
        {
            if (parameters.GameId.HasValue)
                Query.Where(x => x.GameId == parameters.GameId.Value);

            if (parameters.GenreId.HasValue)
                Query.Where(x => x.GenreId == parameters.GenreId.Value);

            Query.Include(x => x.Game)
                 .Include(x => x.Genre);
        }
    }
}