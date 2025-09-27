using GameNest.CatalogService.BLL.DTOs.GameDeveloperRoles;
using GameNest.CatalogService.DAL.Helpers;
using GameNest.CatalogService.Domain.Entities.Parameters;

namespace GameNest.CatalogService.BLL.Services.Interfaces
{
    public interface IGameDeveloperRoleService
    {
        Task<PagedList<GameDeveloperRoleDto>> GetRolesPagedAsync(GameDeveloperRoleParameters parameters, CancellationToken cancellationToken = default);
        Task<GameDeveloperRoleDto?> GetRoleByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<GameDeveloperRoleDto> CreateRoleAsync(GameDeveloperRoleCreateDto createDto, CancellationToken cancellationToken = default);
        Task<GameDeveloperRoleDto> UpdateRoleAsync(Guid id, GameDeveloperRoleUpdateDto updateDto, CancellationToken cancellationToken = default);
        Task DeleteRoleAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
