using GameNest.CatalogService.BLL.DTOs.Games;
using GameNest.CatalogService.DAL.Helpers;
using GameNest.CatalogService.Domain.Entities.Parameters;

namespace GameNest.CatalogService.BLL.Services.Interfaces
{
    public interface IGameService
    {
        Task<PagedList<GameDto>> GetGamesPagedAsync(GameParameters parameters, CancellationToken cancellationToken = default);
        Task<GameDto?> GetGameByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<GameDto> CreateGameAsync(GameCreateDto gameCreateDto, CancellationToken cancellationToken = default);
        Task<GameDto> UpdateGameAsync(Guid id, GameUpdateDto updateDto, CancellationToken cancellationToken = default);
        Task DeleteGameAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
