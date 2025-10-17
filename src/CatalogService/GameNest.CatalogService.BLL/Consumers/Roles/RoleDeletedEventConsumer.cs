using GameNest.CatalogService.BLL.Cache.Services;
using GameNest.Shared.Events.Roles;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GameNest.CatalogService.BLL.Consumers.Roles
{
    public class RoleDeletedEventConsumer : IConsumer<RoleDeletedEvent>
    {
        private readonly IGameCacheInvalidationService _cacheInvalidationService;
        private readonly ILogger<RoleDeletedEventConsumer> _logger;

        public RoleDeletedEventConsumer(
            IGameCacheInvalidationService cacheInvalidationService,
            ILogger<RoleDeletedEventConsumer> logger)
        {
            _cacheInvalidationService = cacheInvalidationService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<RoleDeletedEvent> context)
        {
            var message = context.Message;
            _logger.LogInformation(
                "Received RoleDeletedEvent: RoleId={RoleId}, RoleName={RoleName}",
                message.RoleId, message.RoleName);

            try
            {
                await _cacheInvalidationService.InvalidateAllGamesAsync();

                _logger.LogInformation(
                    "Successfully invalidated game cache after role deletion: RoleId={RoleId}",
                    message.RoleId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to invalidate cache for RoleDeletedEvent: RoleId={RoleId}",
                    message.RoleId);
                throw;
            }
        }
    }
}