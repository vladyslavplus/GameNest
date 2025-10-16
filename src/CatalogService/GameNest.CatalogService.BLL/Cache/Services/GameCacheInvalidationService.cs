using GameNest.ServiceDefaults.Hybrid;
using Microsoft.Extensions.Logging;

namespace GameNest.CatalogService.BLL.Cache.Services
{
    public class GameCacheInvalidationService : IGameCacheInvalidationService
    {
        private readonly IHybridCacheService _cacheService;
        private readonly ILogger<GameCacheInvalidationService> _logger;

        private const string GameCacheKeyPrefix = "game:";
        private const string GamesListPattern = "games:page:*";
        private const string AllGamesPattern = "game:*";

        public GameCacheInvalidationService(
            IHybridCacheService cacheService,
            ILogger<GameCacheInvalidationService> logger)
        {
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task InvalidateGameAsync(Guid gameId)
        {
            try
            {
                string key = $"{GameCacheKeyPrefix}{gameId}";
                await _cacheService.RemoveAsync(key);
                await _cacheService.RemoveByPatternAsync(GamesListPattern);

                _logger.LogInformation("Invalidated cache for game {GameId} and games list", gameId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to invalidate cache for game {GameId}", gameId);
                throw;
            }
        }

        public async Task InvalidateAllGamesAsync()
        {
            try
            {
                await _cacheService.RemoveByPatternAsync(AllGamesPattern);
                await _cacheService.RemoveByPatternAsync(GamesListPattern);

                _logger.LogInformation("Invalidated all game-related caches");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to invalidate all game caches");
                throw;
            }
        }
    }
}