using GameNest.CatalogService.BLL.Cache.Services.Interfaces;
using GameNest.CatalogService.Domain.Entities;
using GameNest.Shared.Events.Developers;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GameNest.CatalogService.BLL.Consumers.Developers
{
    public class DeveloperUpdatedEventConsumer : IConsumer<DeveloperUpdatedEvent>
    {
        private readonly IEntityCacheInvalidationService<Game> _gameCacheInvalidationService;
        private readonly IEntityCacheInvalidationService<GameDeveloperRole> _gameDeveloperRoleCacheInvalidationService;
        private readonly ILogger<DeveloperUpdatedEventConsumer> _logger;

        public DeveloperUpdatedEventConsumer(
            IEntityCacheInvalidationService<Game> gameCacheInvalidationService,
            IEntityCacheInvalidationService<GameDeveloperRole> gameDeveloperRoleCacheInvalidationService,
            ILogger<DeveloperUpdatedEventConsumer> logger)
        {
            _gameCacheInvalidationService = gameCacheInvalidationService;
            _gameDeveloperRoleCacheInvalidationService = gameDeveloperRoleCacheInvalidationService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<DeveloperUpdatedEvent> context)
        {
            var message = context.Message;
            _logger.LogInformation(
                "Received DeveloperUpdatedEvent: DeveloperId={DeveloperId}, OldFullName={OldFullName}, NewFullName={NewFullName}",
                message.DeveloperId, message.OldFullName, message.NewFullName);

            try
            {
                await _gameCacheInvalidationService.InvalidateAllAsync();
                await _gameDeveloperRoleCacheInvalidationService.InvalidateAllAsync();

                _logger.LogInformation(
                    "Successfully invalidated caches after developer update: DeveloperId={DeveloperId}",
                    message.DeveloperId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to invalidate caches for DeveloperUpdatedEvent: DeveloperId={DeveloperId}",
                    message.DeveloperId);
                throw;
            }
        }
    }
}