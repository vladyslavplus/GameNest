using GameNest.CatalogService.BLL.Cache.Services.Interfaces;
using GameNest.CatalogService.Domain.Entities;
using GameNest.Shared.Events.Roles;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GameNest.CatalogService.BLL.Consumers.Roles
{
    public class RoleUpdatedEventConsumer : IConsumer<RoleUpdatedEvent>
    {
        private readonly IEntityCacheInvalidationService<GameDeveloperRole> _cacheInvalidationService;
        private readonly ILogger<RoleUpdatedEventConsumer> _logger;

        public RoleUpdatedEventConsumer(
            IEntityCacheInvalidationService<GameDeveloperRole> cacheInvalidationService,
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
                await _cacheInvalidationService.InvalidateAllAsync();
                _logger.LogInformation("Invalidated GameDeveloperRole cache after role update");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to invalidate cache for RoleUpdatedEvent: RoleId={RoleId}", message.RoleId);
                throw;
            }
        }
    }
}