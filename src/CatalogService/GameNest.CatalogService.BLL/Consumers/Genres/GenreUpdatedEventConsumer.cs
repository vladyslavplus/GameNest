using GameNest.CatalogService.BLL.Cache.Services;
using GameNest.Shared.Events.Genres;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GameNest.CatalogService.BLL.Consumers.Genres
{
    public class GenreUpdatedEventConsumer : IConsumer<GenreUpdatedEvent>
    {
        private readonly IGameCacheInvalidationService _cacheInvalidationService;
        private readonly ILogger<GenreUpdatedEventConsumer> _logger;

        public GenreUpdatedEventConsumer(
            IGameCacheInvalidationService cacheInvalidationService,
            ILogger<GenreUpdatedEventConsumer> logger)
        {
            _cacheInvalidationService = cacheInvalidationService;
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
                await _cacheInvalidationService.InvalidateAllGamesAsync();

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
