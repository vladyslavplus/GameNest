using GameNest.CatalogService.BLL.Cache.Services.Interfaces;
using GameNest.CatalogService.Domain.Entities;
using GameNest.Shared.Events.Developers;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GameNest.CatalogService.BLL.Consumers.Developers
{
    public class DeveloperDeletedEventConsumer : IConsumer<DeveloperDeletedEvent>
    {
        private readonly IEntityCacheInvalidationService<Game> _gameCacheInvalidationService;
        private readonly IEntityCacheInvalidationService<GameDeveloperRole> _gameDeveloperRoleCacheInvalidationService;
        private readonly ILogger<DeveloperDeletedEventConsumer> _logger;

        public DeveloperDeletedEventConsumer(
            IEntityCacheInvalidationService<Game> gameCacheInvalidationService,
            IEntityCacheInvalidationService<GameDeveloperRole> gameDeveloperRoleCacheInvalidationService,
            ILogger<DeveloperDeletedEventConsumer> logger)
        {
            _gameCacheInvalidationService = gameCacheInvalidationService;
            _gameDeveloperRoleCacheInvalidationService = gameDeveloperRoleCacheInvalidationService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<DeveloperDeletedEvent> context)
        {
            var message = context.Message;
            _logger.LogInformation(
                "Received DeveloperDeletedEvent: DeveloperId={DeveloperId}, FullName={FullName}",
                message.DeveloperId, message.FullName);

            try
            {
                await _gameCacheInvalidationService.InvalidateAllAsync();
                await _gameDeveloperRoleCacheInvalidationService.InvalidateAllAsync();

                _logger.LogInformation(
                    "Successfully invalidated caches after developer deletion: DeveloperId={DeveloperId}",
                    message.DeveloperId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to invalidate caches for DeveloperDeletedEvent: DeveloperId={DeveloperId}",
                    message.DeveloperId);
                throw;
            }
        }
    }
}