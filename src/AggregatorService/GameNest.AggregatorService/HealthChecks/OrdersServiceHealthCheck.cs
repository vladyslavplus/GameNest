using GameNest.Grpc.Orders;
using GameNest.ServiceDefaults.Health;

namespace GameNest.AggregatorService.HealthChecks
{
    public class OrdersServiceHealthCheck
        : GrpcServiceHealthCheck<OrderGrpcService.OrderGrpcServiceClient>
    {
        protected override string ServiceName => "OrdersService";
        protected override TimeSpan Timeout => TimeSpan.FromSeconds(2);

        public OrdersServiceHealthCheck(
            OrderGrpcService.OrderGrpcServiceClient client,
            ILogger<OrdersServiceHealthCheck> logger)
            : base(client, logger)
        {
        }

        protected override async Task<bool> PerformHealthCheckAsync(CancellationToken cancellationToken)
        {
            var request = new GetAllOrdersRequest();

            var response = await Client.GetAllOrdersAsync(request, cancellationToken: cancellationToken);
            return response is not null;
        }
    }
}
