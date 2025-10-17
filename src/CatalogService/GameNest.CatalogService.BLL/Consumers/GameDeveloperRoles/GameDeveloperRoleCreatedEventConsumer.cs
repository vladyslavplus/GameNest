using GameNest.CatalogService.BLL.Cache.Services.Interfaces;
using GameNest.CatalogService.Domain.Entities;
using GameNest.Shared.Events.GameDeveloperRoles;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GameNest.CatalogService.BLL.Consumers.GameDeveloperRoles
{
    public class GameDeveloperRoleCreatedEventConsumer : IConsumer<GameDeveloperRoleCreatedEvent>
    {
        private readonly IEntityCacheInvalidationService<GameDeveloperRole> _cacheInvalidationService;
        private readonly ILogger<GameDeveloperRoleCreatedEventConsumer> _logger;

        public GameDeveloperRoleCreatedEventConsumer(
            IEntityCacheInvalidationService<GameDeveloperRole> cacheInvalidationService,
            ILogger<GameDeveloperRoleCreatedEventConsumer> logger)
        {
            _cacheInvalidationService = cacheInvalidationService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<GameDeveloperRoleCreatedEvent> context)
        {
            var message = context.Message;
            _logger.LogInformation(
                "Received GameDeveloperRoleCreatedEvent: GameId={GameId}, DeveloperId={DeveloperId}, RoleId={RoleId}",
                message.GameId, message.DeveloperId, message.RoleId);

            try
            {
                await _cacheInvalidationService.InvalidateByIdAsync(message.GameId);

                _logger.LogInformation(
                    "Successfully invalidated cache after GameDeveloperRole creation: GameId={GameId}",
                    message.GameId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to invalidate cache for GameDeveloperRoleCreatedEvent: GameId={GameId}",
                    message.GameId);
                throw;
            }
        }
    }
}