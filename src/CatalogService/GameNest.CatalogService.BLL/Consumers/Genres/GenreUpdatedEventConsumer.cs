using GameNest.ServiceDefaults.Hybrid;
using GameNest.Shared.Events.Genres;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GameNest.CatalogService.BLL.Consumers.Genres
{
    public class GenreUpdatedEventConsumer : IConsumer<GenreUpdatedEvent>
    {
        private readonly IHybridCacheService _cacheService;
        private readonly ILogger<GenreUpdatedEventConsumer> _logger;

        public GenreUpdatedEventConsumer(
            IHybridCacheService cacheService,
            ILogger<GenreUpdatedEventConsumer> logger)
        {
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<GenreUpdatedEvent> context)
        {
            var message = context.Message;

            _logger.LogInformation(
                "Received GenreUpdatedEvent: GenreId={GenreId}, OldName={OldName}, NewName={NewName}",
                message.GenreId, message.OldName, message.NewName);

            try
            {
                await _cacheService.RemoveByPatternAsync("games:page:*");
                await _cacheService.RemoveByPatternAsync("game:*");

                _logger.LogInformation(
                    "Successfully invalidated game cache after genre update: GenreId={GenreId}",
                    message.GenreId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to invalidate cache for GenreUpdatedEvent: GenreId={GenreId}",
                    message.GenreId);

                throw;
            }
        }
    }
}
