using GameNest.CatalogService.BLL.Cache.Services.Interfaces;
using GameNest.CatalogService.Domain.Entities;
using GameNest.Shared.Events.GamePlatforms;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GameNest.CatalogService.BLL.Consumers.GamePlatforms
{
    public class GamePlatformCreatedEventConsumer : IConsumer<GamePlatformCreatedEvent>
    {
        private readonly IEntityCacheInvalidationService<Game> _cacheInvalidationService;
        private readonly ILogger<GamePlatformCreatedEventConsumer> _logger;

        public GamePlatformCreatedEventConsumer(
            IEntityCacheInvalidationService<Game> cacheInvalidationService,
            ILogger<GamePlatformCreatedEventConsumer> logger)
        {
            _cacheInvalidationService = cacheInvalidationService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<GamePlatformCreatedEvent> context)
        {
            var message = context.Message;
            _logger.LogInformation(
                "Received GamePlatformCreatedEvent: GameId={GameId}, PlatformId={PlatformId}",
                message.GameId, message.PlatformId);

            try
            {
                await _cacheInvalidationService.InvalidateByIdAsync(message.GameId);

                _logger.LogInformation(
                    "Successfully invalidated cache after GamePlatform creation: GameId={GameId}",
                    message.GameId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to invalidate cache for GamePlatformCreatedEvent: GameId={GameId}",
                    message.GameId);
                throw;
            }
        }
    }
}