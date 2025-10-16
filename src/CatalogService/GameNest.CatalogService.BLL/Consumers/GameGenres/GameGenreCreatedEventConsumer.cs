using GameNest.CatalogService.BLL.Cache.Services;
using GameNest.Shared.Events.GameGenres;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GameNest.CatalogService.BLL.Consumers.GameGenres
{
    public class GameGenreCreatedEventConsumer : IConsumer<GameGenreCreatedEvent>
    {
        private readonly IGameCacheInvalidationService _cacheInvalidationService;
        private readonly ILogger<GameGenreCreatedEventConsumer> _logger;

        public GameGenreCreatedEventConsumer(
            IGameCacheInvalidationService cacheInvalidationService,
            ILogger<GameGenreCreatedEventConsumer> logger)
        {
            _cacheInvalidationService = cacheInvalidationService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<GameGenreCreatedEvent> context)
        {
            var message = context.Message;
            _logger.LogInformation(
                "Received GameGenreCreatedEvent: GameId={GameId}, GenreId={GenreId}",
                message.GameId, message.GenreId);

            try
            {
                await _cacheInvalidationService.InvalidateGameAsync(message.GameId);

                _logger.LogInformation(
                    "Successfully invalidated cache after GameGenre creation: GameId={GameId}",
                    message.GameId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to invalidate cache for GameGenreCreatedEvent: GameId={GameId}",
                    message.GameId);
                throw;
            }
        }
    }
}