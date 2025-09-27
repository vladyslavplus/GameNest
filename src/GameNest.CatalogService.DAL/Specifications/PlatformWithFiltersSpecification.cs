using Ardalis.Specification;
using GameNest.CatalogService.Domain.Entities;
using GameNest.CatalogService.Domain.Entities.Parameters;

namespace GameNest.CatalogService.DAL.Specifications
{
    public class PlatformWithFiltersSpecification : Specification<Platform>
    {
        public PlatformWithFiltersSpecification(PlatformParameters parameters)
        {
            if (!string.IsNullOrEmpty(parameters.Name))
                Query.Where(x => x.Name.Contains(parameters.Name));
        }
    }
}