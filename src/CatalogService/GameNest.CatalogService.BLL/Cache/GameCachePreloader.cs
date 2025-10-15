using GameNest.CatalogService.BLL.DTOs.Games;
using GameNest.CatalogService.BLL.Services;
using GameNest.CatalogService.BLL.Services.Interfaces;
using GameNest.CatalogService.Domain.Entities.Parameters;
using GameNest.ServiceDefaults.Hybrid;
using GameNest.ServiceDefaults.Interfaces;
using GameNest.Shared.Helpers;
using Microsoft.Extensions.Logging;

namespace GameNest.CatalogService.BLL.Cache
{
    public class GameCachePreloader : ICachePreloader
    {
        private readonly IHybridCacheService _cacheService;
        private readonly IGameService _gameService;
        private readonly ILogger<GameCachePreloader> _logger;

        public GameCachePreloader(
            IHybridCacheService cacheService,
            IGameService gameService,
            ILogger<GameCachePreloader> logger)
        {
            _cacheService = cacheService;
            _gameService = gameService;
            _logger = logger;
        }

        public async Task PreloadAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting GameCachePreloader warming...");

            try
            {
                for (int page = 1; page <= 3; page++)
                {
                    var parameters = new GameParameters
                    {
                        PageNumber = page,
                        PageSize = 10,
                        OrderBy = "title"
                    };

                    var pagedGames = await _gameService.GetGamesPagedAsync(parameters, cancellationToken);

                    var cacheDto = new PagedListCacheDto<GameDto>
                    {
                        Items = pagedGames.ToList(),
                        TotalCount = pagedGames.TotalCount,
                        PageNumber = pagedGames.CurrentPage,
                        PageSize = pagedGames.PageSize
                    };

                    string cacheKey = GameService.GenerateGamesListCacheKey(parameters);
                    await _cacheService.SetAsync(cacheKey, cacheDto);
                    _logger.LogInformation(
                        "Preloaded games page {Page} with {Count} items into cache with key {CacheKey}.",
                        page, cacheDto.Items.Count, cacheKey);
                }

                _logger.LogInformation("GameCachePreloader completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GameCachePreloader failed during cache warming.");
            }
        }
    }
}