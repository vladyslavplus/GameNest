using GameNest.CatalogService.BLL.Cache.Services;
using GameNest.Shared.Events.GameGenres;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GameNest.CatalogService.BLL.Consumers.GameGenres
{
    public class GameGenreDeletedEventConsumer : IConsumer<GameGenreDeletedEvent>
    {
        private readonly IGameCacheInvalidationService _cacheInvalidationService;
        private readonly ILogger<GameGenreDeletedEventConsumer> _logger;

        public GameGenreDeletedEventConsumer(
            IGameCacheInvalidationService cacheInvalidationService,
            ILogger<GameGenreDeletedEventConsumer> logger)
        {
            _cacheInvalidationService = cacheInvalidationService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<GameGenreDeletedEvent> context)
        {
            var message = context.Message;
            _logger.LogInformation(
                "Received GameGenreDeletedEvent: GameId={GameId}, GenreId={GenreId}",
                message.GameId, message.GenreId);

            try
            {
                await _cacheInvalidationService.InvalidateGameAsync(message.GameId);

                _logger.LogInformation(
                    "Successfully invalidated cache after GameGenre deletion: GameId={GameId}",
                    message.GameId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to invalidate cache for GameGenreDeletedEvent: GameId={GameId}",
                    message.GameId);
                throw;
            }
        }
    }
}