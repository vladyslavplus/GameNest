using GameNest.CatalogService.BLL.Cache.Services.Interfaces;
using GameNest.CatalogService.Domain.Entities;
using GameNest.Shared.Events.Platforms;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GameNest.CatalogService.BLL.Consumers.Platforms
{
    public class PlatformDeletedEventConsumer : IConsumer<PlatformDeletedEvent>
    {
        private readonly IEntityCacheInvalidationService<Game> _cacheInvalidationService;
        private readonly ILogger<PlatformDeletedEventConsumer> _logger;

        public PlatformDeletedEventConsumer(
            IEntityCacheInvalidationService<Game> cacheInvalidationService,
            ILogger<PlatformDeletedEventConsumer> logger)
        {
            _cacheInvalidationService = cacheInvalidationService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PlatformDeletedEvent> context)
        {
            var message = context.Message;
            _logger.LogInformation(
                "Received PlatformDeletedEvent: PlatformId={PlatformId}, PlatformName={PlatformName}",
                message.PlatformId, message.PlatformName);

            try
            {
                await _cacheInvalidationService.InvalidateAllAsync();

                _logger.LogInformation(
                    "Successfully invalidated game cache after platform deletion: PlatformId={PlatformId}",
                    message.PlatformId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to invalidate cache for PlatformDeletedEvent: PlatformId={PlatformId}",
                    message.PlatformId);
                throw;
            }
        }
    }
}