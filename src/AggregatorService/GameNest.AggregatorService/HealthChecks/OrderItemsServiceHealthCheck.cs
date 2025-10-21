using GameNest.Grpc.OrderItems;
using GameNest.ServiceDefaults.Health;

namespace GameNest.AggregatorService.HealthChecks
{
    public class OrderItemsServiceHealthCheck : GrpcServiceHealthCheck<OrderItemGrpcService.OrderItemGrpcServiceClient>
    {
        protected override string ServiceName => "OrderItemsService";
        protected override TimeSpan Timeout => TimeSpan.FromSeconds(2);

        public OrderItemsServiceHealthCheck(
            OrderItemGrpcService.OrderItemGrpcServiceClient client,
            ILogger<OrderItemsServiceHealthCheck> logger)
            : base(client, logger)
        {
        }

        protected override async Task<bool> PerformHealthCheckAsync(CancellationToken cancellationToken)
        {
            var request = new GetAllOrderItemsRequest();

            var response = await Client.GetAllOrderItemsAsync(request, cancellationToken: cancellationToken);
            return response is not null;
        }
    }
}
