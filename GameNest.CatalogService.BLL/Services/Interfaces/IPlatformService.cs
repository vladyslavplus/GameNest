using GameNest.CatalogService.BLL.DTOs.Platforms;
using GameNest.CatalogService.DAL.Helpers;
using GameNest.CatalogService.Domain.Entities.Parameters;

namespace GameNest.CatalogService.BLL.Services.Interfaces
{
    public interface IPlatformService
    {
        Task<PagedList<PlatformDto>> GetPlatformsPagedAsync(PlatformParameters parameters, CancellationToken cancellationToken = default);
        Task<PlatformDto?> GetPlatformByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PlatformDto> CreatePlatformAsync(PlatformCreateDto dto, CancellationToken cancellationToken = default);
        Task<PlatformDto> UpdatePlatformAsync(Guid id, PlatformUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeletePlatformAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
