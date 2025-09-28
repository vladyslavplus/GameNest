using Ardalis.Specification;
using GameNest.CatalogService.Domain.Entities;
using GameNest.CatalogService.Domain.Entities.Parameters;

namespace GameNest.CatalogService.DAL.Specifications
{
    public class GameWithFiltersSpecification : Specification<Game>
    {
        public GameWithFiltersSpecification(GameParameters parameters)
        {
            if (!string.IsNullOrEmpty(parameters.Title))
                Query.Where(g => g.Title.Contains(parameters.Title));

            if (parameters.MinPrice.HasValue)
                Query.Where(g => g.Price >= parameters.MinPrice.Value);

            if (parameters.MaxPrice.HasValue)
                Query.Where(g => g.Price <= parameters.MaxPrice.Value);

            if (parameters.PublisherId.HasValue)
                Query.Where(g => g.PublisherId == parameters.PublisherId.Value);

            Query.Include(g => g.Publisher)
                 .Include(g => g.GameGenres)
                     .ThenInclude(gg => gg.Genre)
                 .Include(g => g.GamePlatforms)
                     .ThenInclude(gp => gp.Platform)
                 .Include(g => g.GameDeveloperRoles)        
                     .ThenInclude(gdr => gdr.Developer);    
        }
    }
}