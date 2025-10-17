using GameNest.Shared.Events.Roles;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GameNest.CatalogService.BLL.Consumers.Roles
{
    public class RoleUpdatedEventConsumer : IConsumer<RoleUpdatedEvent>
    {
        private readonly ILogger<RoleUpdatedEventConsumer> _logger;

        public RoleUpdatedEventConsumer(ILogger<RoleUpdatedEventConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<RoleUpdatedEvent> context)
        {
            var message = context.Message;
            _logger.LogInformation(
                "Received RoleUpdatedEvent: RoleId={RoleId}, OldName={OldName}, NewName={NewName}",
                message.RoleId, message.OldName, message.NewName);

            await Task.CompletedTask; // no cache invalidation yet
        }
    }
}