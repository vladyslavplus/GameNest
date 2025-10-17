using GameNest.CatalogService.BLL.Cache.Services;
using GameNest.Shared.Events.Developers;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GameNest.CatalogService.BLL.Consumers.Developers
{
    public class DeveloperDeletedEventConsumer : IConsumer<DeveloperDeletedEvent>
    {
        private readonly IGameCacheInvalidationService _cacheInvalidationService;
        private readonly ILogger<DeveloperDeletedEventConsumer> _logger;

        public DeveloperDeletedEventConsumer(
            IGameCacheInvalidationService cacheInvalidationService,
            ILogger<DeveloperDeletedEventConsumer> logger)
        {
            _cacheInvalidationService = cacheInvalidationService;
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
                await _cacheInvalidationService.InvalidateAllGamesAsync();

                _logger.LogInformation(
                    "Successfully invalidated game cache after developer deletion: DeveloperId={DeveloperId}",
                    message.DeveloperId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to invalidate cache for DeveloperDeletedEvent: DeveloperId={DeveloperId}",
                    message.DeveloperId);
                throw;
            }
        }
    }
}