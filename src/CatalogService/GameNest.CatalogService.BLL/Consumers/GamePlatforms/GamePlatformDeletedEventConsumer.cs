using GameNest.CatalogService.BLL.Cache.Services.Interfaces;
using GameNest.CatalogService.Domain.Entities;
using GameNest.Shared.Events.GamePlatforms;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GameNest.CatalogService.BLL.Consumers.GamePlatforms
{
    public class GamePlatformDeletedEventConsumer : IConsumer<GamePlatformDeletedEvent>
    {
        private readonly IEntityCacheInvalidationService<Game> _cacheInvalidationService;
        private readonly ILogger<GamePlatformDeletedEventConsumer> _logger;

        public GamePlatformDeletedEventConsumer(
            IEntityCacheInvalidationService<Game> cacheInvalidationService,
            ILogger<GamePlatformDeletedEventConsumer> logger)
        {
            _cacheInvalidationService = cacheInvalidationService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<GamePlatformDeletedEvent> context)
        {
            var message = context.Message;
            _logger.LogInformation(
                "Received GamePlatformDeletedEvent: GameId={GameId}, PlatformId={PlatformId}",
                message.GameId, message.PlatformId);

            try
            {
                await _cacheInvalidationService.InvalidateByIdAsync(message.GameId);

                _logger.LogInformation(
                    "Successfully invalidated cache after GamePlatform deletion: GameId={GameId}",
                    message.GameId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to invalidate cache for GamePlatformDeletedEvent: GameId={GameId}",
                    message.GameId);
                throw;
            }
        }
    }
}