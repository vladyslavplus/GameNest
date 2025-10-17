using GameNest.CatalogService.BLL.Cache.Services.Interfaces;
using GameNest.CatalogService.Domain.Entities;
using GameNest.ServiceDefaults.Hybrid;
using Microsoft.Extensions.Logging;

namespace GameNest.CatalogService.BLL.Cache.Services
{
    public class GameDeveloperRoleCacheInvalidationService : IEntityCacheInvalidationService<GameDeveloperRole>
    {
        private readonly IHybridCacheService _cacheService;
        private readonly ILogger<GameDeveloperRoleCacheInvalidationService> _logger;

        private const string CacheKeyPrefix = "gamedevrole:";
        private const string ListPattern = "gamedevroles:page:*";
        private const string AllPattern = "gamedevrole:*";

        public GameDeveloperRoleCacheInvalidationService(
            IHybridCacheService cacheService,
            ILogger<GameDeveloperRoleCacheInvalidationService> logger)
        {
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task InvalidateByIdAsync(Guid entityId)
        {
            try
            {
                string key = $"{CacheKeyPrefix}{entityId}";
                await _cacheService.RemoveAsync(key);
                await _cacheService.RemoveByPatternAsync(ListPattern);

                _logger.LogInformation("Invalidated cache for GameDeveloperRole {EntityId} and list cache", entityId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to invalidate cache for GameDeveloperRole {EntityId}", entityId);
                throw;
            }
        }

        public async Task InvalidateAllAsync()
        {
            try
            {
                await _cacheService.RemoveByPatternAsync(AllPattern);
                await _cacheService.RemoveByPatternAsync(ListPattern);

                _logger.LogInformation("Invalidated all GameDeveloperRole-related caches");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to invalidate all GameDeveloperRole caches");
                throw;
            }
        }
    }
}