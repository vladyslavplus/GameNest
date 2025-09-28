using GameNest.CatalogService.BLL.DTOs.Roles;
using GameNest.CatalogService.DAL.Helpers;
using GameNest.CatalogService.Domain.Entities.Parameters;

namespace GameNest.CatalogService.BLL.Services.Interfaces
{
    public interface IRoleService
    {
        Task<PagedList<RoleDto>> GetRolesPagedAsync(RoleParameters parameters, CancellationToken cancellationToken = default);
        Task<RoleDto?> GetRoleByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<RoleDto> CreateRoleAsync(RoleCreateDto createDto, CancellationToken cancellationToken = default);
        Task<RoleDto> UpdateRoleAsync(Guid id, RoleUpdateDto updateDto, CancellationToken cancellationToken = default);
        Task DeleteRoleAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
