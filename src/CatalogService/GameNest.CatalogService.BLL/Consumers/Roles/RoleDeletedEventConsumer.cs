using GameNest.CatalogService.BLL.Cache.Services.Interfaces;
using GameNest.CatalogService.Domain.Entities;
using GameNest.Shared.Events.Roles;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GameNest.CatalogService.BLL.Consumers.Roles
{
    public class RoleDeletedEventConsumer : IConsumer<RoleDeletedEvent>
    {
        private readonly IEntityCacheInvalidationService<GameDeveloperRole> _cacheInvalidationService;
        private readonly ILogger<RoleDeletedEventConsumer> _logger;

        public RoleDeletedEventConsumer(
            IEntityCacheInvalidationService<GameDeveloperRole> cacheInvalidationService,
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
                await _cacheInvalidationService.InvalidateAllAsync();
                _logger.LogInformation("Invalidated GameDeveloperRole cache after role deletion");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to invalidate cache for RoleDeletedEvent: RoleId={RoleId}", message.RoleId);
                throw;
            }
        }
    }
}