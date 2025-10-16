using GameNest.CatalogService.BLL.Cache.Services;
using GameNest.Shared.Events.Platforms;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GameNest.CatalogService.BLL.Consumers.Platforms
{
    public class PlatformUpdatedEventConsumer : IConsumer<PlatformUpdatedEvent>
    {
        private readonly IGameCacheInvalidationService _cacheInvalidationService;
        private readonly ILogger<PlatformUpdatedEventConsumer> _logger;

        public PlatformUpdatedEventConsumer(
            IGameCacheInvalidationService cacheInvalidationService,
            ILogger<PlatformUpdatedEventConsumer> logger)
        {
            _cacheInvalidationService = cacheInvalidationService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PlatformUpdatedEvent> context)
        {
            var message = context.Message;
            _logger.LogInformation(
                "Received PlatformUpdatedEvent: PlatformId={PlatformId}, OldName={OldName}, NewName={NewName}",
                message.PlatformId, message.OldName, message.NewName);

            try
            {
                await _cacheInvalidationService.InvalidateAllGamesAsync();

                _logger.LogInformation(
                    "Successfully invalidated game cache after platform update: PlatformId={PlatformId}",
                    message.PlatformId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to invalidate cache for PlatformUpdatedEvent: PlatformId={PlatformId}",
                    message.PlatformId);
                throw;
            }
        }
    }
}