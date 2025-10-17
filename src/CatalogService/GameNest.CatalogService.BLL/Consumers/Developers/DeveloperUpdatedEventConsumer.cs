using GameNest.CatalogService.BLL.Cache.Services;
using GameNest.Shared.Events.Developers;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GameNest.CatalogService.BLL.Consumers.Developers
{
    public class DeveloperUpdatedEventConsumer : IConsumer<DeveloperUpdatedEvent>
    {
        private readonly IGameCacheInvalidationService _cacheInvalidationService;
        private readonly ILogger<DeveloperUpdatedEventConsumer> _logger;

        public DeveloperUpdatedEventConsumer(
            IGameCacheInvalidationService cacheInvalidationService,
            ILogger<DeveloperUpdatedEventConsumer> logger)
        {
            _cacheInvalidationService = cacheInvalidationService;
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
                await _cacheInvalidationService.InvalidateAllGamesAsync();

                _logger.LogInformation(
                    "Successfully invalidated game cache after developer update: DeveloperId={DeveloperId}",
                    message.DeveloperId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to invalidate cache for DeveloperUpdatedEvent: DeveloperId={DeveloperId}",
                    message.DeveloperId);
                throw;
            }
        }
    }
}