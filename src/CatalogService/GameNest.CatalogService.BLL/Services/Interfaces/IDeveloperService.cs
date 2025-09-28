using GameNest.CatalogService.BLL.DTOs.Developers;
using GameNest.CatalogService.DAL.Helpers;
using GameNest.CatalogService.Domain.Entities.Parameters;

namespace GameNest.CatalogService.BLL.Services.Interfaces
{
    public interface IDeveloperService
    {
        Task<PagedList<DeveloperDto>> GetDevelopersPagedAsync(DeveloperParameters parameters, CancellationToken cancellationToken = default);
        Task<DeveloperDto?> GetDeveloperByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<DeveloperDto> CreateDeveloperAsync(DeveloperCreateDto developerCreateDto, CancellationToken cancellationToken = default);
        Task<DeveloperDto> UpdateDeveloperAsync(Guid id, DeveloperUpdateDto updateDto, CancellationToken cancellationToken = default);
        Task DeleteDeveloperAsync(Guid id, CancellationToken cancellationToken = default);
    }
}