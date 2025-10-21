using GameNest.Grpc.Games;
using GameNest.ServiceDefaults.Health;

namespace GameNest.AggregatorService.HealthChecks
{
    public class CatalogServiceHealthCheck
        : GrpcServiceHealthCheck<GameGrpcService.GameGrpcServiceClient>
    {
        protected override string ServiceName => "CatalogService";

        public CatalogServiceHealthCheck(
            GameGrpcService.GameGrpcServiceClient client,
            ILogger<CatalogServiceHealthCheck> logger)
            : base(client, logger)
        {
        }

        protected override async Task<bool> PerformHealthCheckAsync(CancellationToken cancellationToken)
        {
            var request = new GetGamesRequest
            {
                PageNumber = 1,
                PageSize = 1
            };

            var response = await Client.GetGamesPagedAsync(request, cancellationToken: cancellationToken);
            return response is not null;
        }
    }
}
