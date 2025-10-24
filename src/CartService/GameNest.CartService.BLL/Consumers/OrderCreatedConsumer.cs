using GameNest.CartService.BLL.Interfaces;
using GameNest.Shared.Events.Orders;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GameNest.CartService.BLL.Consumers
{
    public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
    {
        private readonly ICartService _cartService;
        private readonly ILogger<OrderCreatedConsumer> _logger;

        public OrderCreatedConsumer(ICartService cartService, ILogger<OrderCreatedConsumer> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            var message = context.Message;

            _logger.LogInformation(
                "Received OrderCreatedEvent for CustomerId {CustomerId}, OrderId {OrderId}. Clearing cart...",
                message.CustomerId, message.OrderId);

            try
            {
                await _cartService.ClearCartAsync(message.CustomerId);
                _logger.LogInformation("Successfully cleared cart for user {CustomerId} after order {OrderId}.",
                    message.CustomerId, message.OrderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to clear cart for user {CustomerId} after order {OrderId}.",
                    message.CustomerId, message.OrderId);
            }
        }
    }
}
