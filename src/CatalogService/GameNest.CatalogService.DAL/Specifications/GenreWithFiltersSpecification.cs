using Ardalis.Specification;
using GameNest.CatalogService.Domain.Entities;
using GameNest.CatalogService.Domain.Entities.Parameters;

namespace GameNest.CatalogService.DAL.Specifications
{
    public class GenreWithFiltersSpecification : Specification<Genre>
    {
        public GenreWithFiltersSpecification(GenreParameters parameters)
        {
            if (!string.IsNullOrEmpty(parameters.Name))
                Query.Where(x => x.Name.Contains(parameters.Name));
        }
    }
}