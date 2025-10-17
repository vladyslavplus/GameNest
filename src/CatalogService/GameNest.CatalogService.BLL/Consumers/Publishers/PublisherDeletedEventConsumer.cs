using GameNest.CatalogService.BLL.Cache.Services.Interfaces;
using GameNest.CatalogService.Domain.Entities;
using GameNest.Shared.Events.Publishers;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GameNest.CatalogService.BLL.Consumers.Publishers
{
    public class PublisherDeletedEventConsumer : IConsumer<PublisherDeletedEvent>
    {
        private readonly IEntityCacheInvalidationService<Game> _cacheInvalidationService;
        private readonly ILogger<PublisherDeletedEventConsumer> _logger;

        public PublisherDeletedEventConsumer(
            IEntityCacheInvalidationService<Game> cacheInvalidationService,
            ILogger<PublisherDeletedEventConsumer> logger)
        {
            _cacheInvalidationService = cacheInvalidationService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PublisherDeletedEvent> context)
        {
            var message = context.Message;
            _logger.LogInformation(
                "Received PublisherDeletedEvent: PublisherId={PublisherId}, PublisherName={PublisherName}",
                message.PublisherId, message.PublisherName);

            try
            {
                await _cacheInvalidationService.InvalidateAllAsync();

                _logger.LogInformation(
                    "Successfully invalidated game cache after publisher deletion: PublisherId={PublisherId}",
                    message.PublisherId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to invalidate cache for PublisherDeletedEvent: PublisherId={PublisherId}",
                    message.PublisherId);
                throw;
            }
        }
    }
}