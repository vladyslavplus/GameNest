using GameNest.CatalogService.DAL.Data;
using GameNest.CatalogService.DAL.Extensions;
using GameNest.CatalogService.DAL.Helpers;
using GameNest.CatalogService.DAL.Repositories.Interfaces;
using GameNest.CatalogService.DAL.Specifications;
using GameNest.CatalogService.Domain.Entities;
using GameNest.CatalogService.Domain.Entities.Parameters;

namespace GameNest.CatalogService.DAL.Repositories
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(CatalogDbContext context)
            : base(context)
        {
        }

        public async Task<PagedList<Role>> GetRolesPagedAsync(
            RoleParameters parameters,
            ISortHelper<Role>? sortHelper = null,
            CancellationToken cancellationToken = default)
        {
            var spec = new RoleWithFiltersSpecification(parameters);
            var query = ApplySpecification(spec)
                .ApplySorting(parameters.OrderBy, sortHelper);

            return await query.ToPagedListAsync(parameters, cancellationToken);
        }
    }
}
