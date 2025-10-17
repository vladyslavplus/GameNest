using GameNest.CatalogService.BLL.Cache.Services;
using GameNest.Shared.Events.Roles;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GameNest.CatalogService.BLL.Consumers.Roles
{
    public class RoleUpdatedEventConsumer : IConsumer<RoleUpdatedEvent>
    {
        private readonly IGameCacheInvalidationService _cacheInvalidationService;
        private readonly ILogger<RoleUpdatedEventConsumer> _logger;

        public RoleUpdatedEventConsumer(
            IGameCacheInvalidationService cacheInvalidationService,
            ILogger<RoleUpdatedEventConsumer> logger)
        {
            _cacheInvalidationService = cacheInvalidationService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<RoleUpdatedEvent> context)
        {
            var message = context.Message;
            _logger.LogInformation(
                "Received RoleUpdatedEvent: RoleId={RoleId}, OldName={OldName}, NewName={NewName}",
                message.RoleId, message.OldName, message.NewName);

            try
            {
                await _cacheInvalidationService.InvalidateAllGamesAsync();

                _logger.LogInformation(
                    "Successfully invalidated game cache after role update: RoleId={RoleId}",
                    message.RoleId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to invalidate cache for RoleUpdatedEvent: RoleId={RoleId}",
                    message.RoleId);
                throw;
            }
        }
    }
}