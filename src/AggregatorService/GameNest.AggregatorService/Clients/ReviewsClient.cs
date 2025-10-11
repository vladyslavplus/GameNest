using GameNest.AggregatorService.DTOs.Reviews;

namespace GameNest.AggregatorService.Clients
{
    public class ReviewsClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ReviewsClient> _logger;

        public ReviewsClient(HttpClient httpClient, ILogger<ReviewsClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<ReviewDto>?> GetReviewsByGameIdAsync(string gameId, CancellationToken ct)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/reviews?gameId={gameId}", ct);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to fetch reviews for game {GameId}: {StatusCode}", gameId, response.StatusCode);
                    return null;
                }

                var reviews = await response.Content.ReadFromJsonAsync<IEnumerable<ReviewDto>>(cancellationToken: ct);

                return reviews;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching reviews for game {GameId}", gameId);
                return null;
            }
        }

        public async Task<ReviewDto?> GetReviewByIdAsync(string reviewId, CancellationToken ct)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/reviews/{reviewId}", ct);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to fetch review {ReviewId}: {StatusCode}", reviewId, response.StatusCode);
                    return null;
                }

                var review = await response.Content.ReadFromJsonAsync<ReviewDto>(cancellationToken: ct);
                return review;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching review {ReviewId}", reviewId);
                return null;
            }
        }
    }
}
