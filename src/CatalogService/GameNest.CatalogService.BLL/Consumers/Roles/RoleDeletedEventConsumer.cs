using GameNest.Shared.Events.Roles;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GameNest.CatalogService.BLL.Consumers.Roles
{
    public class RoleDeletedEventConsumer : IConsumer<RoleDeletedEvent>
    {
        private readonly ILogger<RoleDeletedEventConsumer> _logger;

        public RoleDeletedEventConsumer(ILogger<RoleDeletedEventConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<RoleDeletedEvent> context)
        {
            var message = context.Message;
            _logger.LogInformation(
                "Received RoleDeletedEvent: RoleId={RoleId}, RoleName={RoleName}",
                message.RoleId, message.RoleName);

            await Task.CompletedTask; // no cache invalidation yet
        }
    }
}