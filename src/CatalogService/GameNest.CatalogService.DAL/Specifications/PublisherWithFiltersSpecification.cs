using Ardalis.Specification;
using GameNest.CatalogService.Domain.Entities;
using GameNest.CatalogService.Domain.Entities.Parameters;

namespace GameNest.CatalogService.DAL.Specifications
{
    public class PublisherWithFiltersSpecification : Specification<Publisher>
    {
        public PublisherWithFiltersSpecification(PublisherParameters parameters)
        {
            if (!string.IsNullOrEmpty(parameters.Name))
                Query.Where(x => x.Name.Contains(parameters.Name));

            if (!string.IsNullOrEmpty(parameters.Type))
                Query.Where(x => x.Type.Contains(parameters.Type));

            if (!string.IsNullOrEmpty(parameters.Country))
                Query.Where(x => x.Country != null && x.Country.Contains(parameters.Country));

            if (!string.IsNullOrEmpty(parameters.Phone))
                Query.Where(x => x.Phone != null && x.Phone.Contains(parameters.Phone));

            Query.Include(x => x.Games);
        }
    }
}
