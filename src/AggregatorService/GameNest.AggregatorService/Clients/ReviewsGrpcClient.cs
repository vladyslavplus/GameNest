using GameNest.AggregatorService.DTOs.Reviews;
using GameNest.Grpc.Reviews;
using Grpc.Core;

namespace GameNest.AggregatorService.Clients
{
    public class ReviewsGrpcClient
    {
        private readonly ReviewGrpcService.ReviewGrpcServiceClient _client;
        private readonly ILogger<ReviewsGrpcClient> _logger;

        public ReviewsGrpcClient(
            ReviewGrpcService.ReviewGrpcServiceClient client,
            ILogger<ReviewsGrpcClient> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<IEnumerable<Review>?> GetReviewsByGameIdAsync(string gameId, CancellationToken ct)
        {
            try
            {
                var request = new GetReviewsRequest
                {
                    GameId = gameId,
                    PageNumber = 1,
                    PageSize = 1000 // Або скільки вам потрібно
                };

                var response = await _client.GetReviewsAsync(request, cancellationToken: ct);
                return response.Items;
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "gRPC error while fetching reviews for game {GameId}", gameId);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching reviews for game {GameId}", gameId);
                return null;
            }
        }

        public async Task<Review?> GetReviewByIdAsync(string reviewId, CancellationToken ct)
        {
            try
            {
                var request = new GetReviewByIdRequest
                {
                    Id = reviewId
                };

                var response = await _client.GetReviewByIdAsync(request, cancellationToken: ct);
                return response.Review;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                _logger.LogWarning(ex, "Review {ReviewId} not found", reviewId);
                return null;
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "gRPC error while fetching review {ReviewId}", reviewId);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching review {ReviewId}", reviewId);
                return null;
            }
        }
    }
}
