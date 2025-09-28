using Ardalis.Specification;
using GameNest.CatalogService.Domain.Entities;
using GameNest.CatalogService.Domain.Entities.Parameters;

namespace GameNest.CatalogService.DAL.Specifications
{
    public class DeveloperWithFiltersSpecification : Specification<Developer>
    {
        public DeveloperWithFiltersSpecification(DeveloperParameters parameters)
        {
            if (!string.IsNullOrEmpty(parameters.FullName))
                Query.Where(d => d.FullName.Contains(parameters.FullName));

            if (!string.IsNullOrEmpty(parameters.Email))
                Query.Where(d => d.Email != null && d.Email.Contains(parameters.Email));

            if (!string.IsNullOrEmpty(parameters.Country))
                Query.Where(d => d.Country != null && d.Country.Contains(parameters.Country));

            Query.Include(d => d.GameDeveloperRoles)
                 .ThenInclude(gdr => gdr.Game);

            Query.Include(d => d.GameDeveloperRoles)
                 .ThenInclude(gdr => gdr.Role);
        }
    }
}