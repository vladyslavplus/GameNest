using GameNest.CatalogService.BLL.DTOs.GamePlatforms;
using GameNest.CatalogService.DAL.Helpers;
using GameNest.CatalogService.Domain.Entities.Parameters;

namespace GameNest.CatalogService.BLL.Services.Interfaces
{
    public interface IGamePlatformService
    {
        Task<PagedList<GamePlatformDto>> GetGamePlatformsPagedAsync(GamePlatformParameters parameters, CancellationToken cancellationToken = default);
        Task<GamePlatformDto?> GetGamePlatformByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<GamePlatformDto> CreateGamePlatformAsync(GamePlatformCreateDto createDto, CancellationToken cancellationToken = default);
        Task<GamePlatformDto> UpdateGamePlatformAsync(Guid id, GamePlatformUpdateDto updateDto, CancellationToken cancellationToken = default);
        Task DeleteGamePlatformAsync( Guid id, CancellationToken cancellationToken = default);
    }
}
