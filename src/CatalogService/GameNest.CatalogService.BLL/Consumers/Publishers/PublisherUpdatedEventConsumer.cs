using GameNest.CatalogService.BLL.Cache.Services;
using GameNest.Shared.Events.Publishers;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GameNest.CatalogService.BLL.Consumers.Publishers
{
    public class PublisherUpdatedEventConsumer : IConsumer<PublisherUpdatedEvent>
    {
        private readonly IGameCacheInvalidationService _cacheInvalidationService;
        private readonly ILogger<PublisherUpdatedEventConsumer> _logger;

        public PublisherUpdatedEventConsumer(
            IGameCacheInvalidationService cacheInvalidationService,
            ILogger<PublisherUpdatedEventConsumer> logger)
        {
            _cacheInvalidationService = cacheInvalidationService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PublisherUpdatedEvent> context)
        {
            var message = context.Message;
            _logger.LogInformation(
                "Received PublisherUpdatedEvent: PublisherId={PublisherId}, OldName={OldName}, NewName={NewName}",
                message.PublisherId, message.OldName, message.NewName);

            try
            {
                await _cacheInvalidationService.InvalidateAllGamesAsync();

                _logger.LogInformation(
                    "Successfully invalidated game cache after publisher update: PublisherId={PublisherId}",
                    message.PublisherId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to invalidate cache for PublisherUpdatedEvent: PublisherId={PublisherId}",
                    message.PublisherId);
                throw;
            }
        }
    }
}