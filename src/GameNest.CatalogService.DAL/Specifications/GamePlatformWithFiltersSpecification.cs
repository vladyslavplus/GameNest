using Ardalis.Specification;
using GameNest.CatalogService.Domain.Entities;
using GameNest.CatalogService.Domain.Entities.Parameters;

namespace GameNest.CatalogService.DAL.Specifications
{
    public class GamePlatformWithFiltersSpecification : Specification<GamePlatform>
    {
        public GamePlatformWithFiltersSpecification(GamePlatformParameters parameters)
        {
            if (parameters.GameId.HasValue)
                Query.Where(x => x.GameId == parameters.GameId.Value);

            if (parameters.PlatformId.HasValue)
                Query.Where(x => x.PlatformId == parameters.PlatformId.Value);

            Query.Include(x => x.Game)
                 .Include(x => x.Platform);
        }
    }
}