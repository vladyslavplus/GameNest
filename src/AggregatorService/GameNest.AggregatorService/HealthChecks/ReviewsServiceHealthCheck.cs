using GameNest.Grpc.Reviews;
using GameNest.ServiceDefaults.Health;

namespace GameNest.AggregatorService.HealthChecks
{
    public class ReviewsServiceHealthCheck
        : GrpcServiceHealthCheck<ReviewGrpcService.ReviewGrpcServiceClient>
    {
        protected override string ServiceName => "ReviewsService";

        public ReviewsServiceHealthCheck(
            ReviewGrpcService.ReviewGrpcServiceClient client,
            ILogger<ReviewsServiceHealthCheck> logger)
            : base(client, logger)
        {
        }

        protected override async Task<bool> PerformHealthCheckAsync(CancellationToken cancellationToken)
        {
            var request = new GetReviewsRequest
            {
                PageNumber = 1,
                PageSize = 1
            };

            var response = await Client.GetReviewsAsync(request, cancellationToken: cancellationToken);
            return response is not null;
        }
    }
}
