using Ardalis.Specification;
using GameNest.CatalogService.Domain.Entities;
using GameNest.CatalogService.Domain.Entities.Parameters;

namespace GameNest.CatalogService.DAL.Specifications
{
    public class RoleWithFiltersSpecification : Specification<Role>
    {
        public RoleWithFiltersSpecification(RoleParameters parameters)
        {
            if (!string.IsNullOrEmpty(parameters.Name))
                Query.Where(x => x.Name.Contains(parameters.Name));
        }
    }
}