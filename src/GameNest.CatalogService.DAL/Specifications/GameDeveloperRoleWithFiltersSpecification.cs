using Ardalis.Specification;
using GameNest.CatalogService.Domain.Entities;
using GameNest.CatalogService.Domain.Entities.Parameters;

namespace GameNest.CatalogService.DAL.Specifications
{
    public class GameDeveloperRoleWithFiltersSpecification : Specification<GameDeveloperRole>
    {
        public GameDeveloperRoleWithFiltersSpecification(GameDeveloperRoleParameters parameters)
        {
            if (parameters.GameId.HasValue)
                Query.Where(x => x.GameId == parameters.GameId.Value);

            if (parameters.DeveloperId.HasValue)
                Query.Where(x => x.DeveloperId == parameters.DeveloperId.Value);

            if (parameters.RoleId.HasValue)
                Query.Where(x => x.RoleId == parameters.RoleId.Value);

            if (!string.IsNullOrEmpty(parameters.Seniority))
                Query.Where(x => x.Seniority.Contains(parameters.Seniority));

            Query.Include(x => x.Game)
                 .Include(x => x.Developer)
                 .Include(x => x.Role);
        }
    }
}
