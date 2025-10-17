using GameNest.CatalogService.BLL.Cache.Services;
using GameNest.Shared.Events.GameDeveloperRoles;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GameNest.CatalogService.BLL.Consumers.GameDeveloperRoles
{
    public class GameDeveloperRoleDeletedEventConsumer : IConsumer<GameDeveloperRoleDeletedEvent>
    {
        private readonly IGameCacheInvalidationService _cacheInvalidationService;
        private readonly ILogger<GameDeveloperRoleDeletedEventConsumer> _logger;

        public GameDeveloperRoleDeletedEventConsumer(
            IGameCacheInvalidationService cacheInvalidationService,
            ILogger<GameDeveloperRoleDeletedEventConsumer> logger)
        {
            _cacheInvalidationService = cacheInvalidationService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<GameDeveloperRoleDeletedEvent> context)
        {
            var message = context.Message;
            _logger.LogInformation(
                "Received GameDeveloperRoleDeletedEvent: GameId={GameId}, DeveloperId={DeveloperId}, RoleId={RoleId}",
                message.GameId, message.DeveloperId, message.RoleId);

            try
            {
                await _cacheInvalidationService.InvalidateGameAsync(message.GameId);

                _logger.LogInformation(
                    "Successfully invalidated cache after GameDeveloperRole deletion: GameId={GameId}",
                    message.GameId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to invalidate cache for GameDeveloperRoleDeletedEvent: GameId={GameId}",
                    message.GameId);
                throw;
            }
        }
    }
}