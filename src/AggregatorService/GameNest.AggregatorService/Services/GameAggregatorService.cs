using GameNest.AggregatorService.Clients;
using GameNest.AggregatorService.DTOs.Aggregated;
using GameNest.AggregatorService.DTOs.Reviews;

namespace GameNest.AggregatorService.Services
{
    public class GameAggregatorService
    {
        private readonly CatalogClient _catalogClient;
        private readonly ReviewsClient _reviewsClient;
        private readonly ILogger<GameAggregatorService> _logger;

        public GameAggregatorService(
            CatalogClient catalogClient,
            ReviewsClient reviewsClient,
            ILogger<GameAggregatorService> logger)
        {
            _catalogClient = catalogClient;
            _reviewsClient = reviewsClient;
            _logger = logger;
        }

        public async Task<IEnumerable<AggregatedGameDto>?> GetAllAggregatedGamesAsync(CancellationToken ct)
        {
            var games = await _catalogClient.GetGamesAsync(ct);
            if (games is null || !games.Any())
            {
                _logger.LogWarning("No games found in catalog");
                return null;
            }

            var aggregatedGames = new List<AggregatedGameDto>();

            foreach (var game in games)
            {
                var warnings = new List<string>();

                var reviews = await _reviewsClient.GetReviewsByGameIdAsync(game.Id.ToString(), ct);
                var reviewList = reviews?.ToList() ?? new List<ReviewDto>();

                if (reviews is null)
                {
                    warnings.Add($"Failed to fetch reviews for game '{game.Title}'.");
                    _logger.LogWarning("Could not fetch reviews for GameId {GameId}", game.Id);
                }

                var averageRating = reviewList.Any()
                    ? reviewList.Average(r => r.Rating?.Value ?? 0)
                    : (double?)null;

                var dto = new AggregatedGameDto
                {
                    Id = game.Id,
                    Title = game.Title,
                    Description = game.Description,
                    ReleaseDate = game.ReleaseDate,
                    Price = game.Price,
                    Publisher = game.Publisher,
                    Genres = game.Genres,
                    Platforms = game.Platforms,

                    Reviews = reviewList,
                    ReviewCount = reviewList.Count,
                    AverageRating = averageRating,

                    PartialData = reviews is null,
                    Warnings = warnings.ToArray(),
                    ResponseTimestamp = DateTime.UtcNow,
                    Summary = $"Game '{game.Title}' — {reviewList.Count} reviews, avg rating: {averageRating?.ToString("0.0") ?? "N/A"}"
                };

                aggregatedGames.Add(dto);
            }

            _logger.LogInformation("Successfully aggregated data for {GameCount} games.", aggregatedGames.Count);
            return aggregatedGames;
        }

        public async Task<AggregatedGameDto?> GetAggregatedGameByIdAsync(Guid gameId, CancellationToken ct)
        {
            var game = await _catalogClient.GetGameByIdAsync(gameId, ct);
            if (game is null)
            {
                _logger.LogWarning("Game with ID {GameId} not found", gameId);
                return null;
            }

            var warnings = new List<string>();

            var reviews = await _reviewsClient.GetReviewsByGameIdAsync(game.Id.ToString(), ct);
            var reviewList = reviews?.ToList() ?? new List<ReviewDto>();

            if (reviews is null)
            {
                warnings.Add($"Failed to fetch reviews for game '{game.Title}'.");
                _logger.LogWarning("Could not fetch reviews for GameId {GameId}", game.Id);
            }

            var averageRating = reviewList.Any()
                ? reviewList.Average(r => r.Rating?.Value ?? 0)
                : (double?)null;

            var dto = new AggregatedGameDto
            {
                Id = game.Id,
                Title = game.Title,
                Description = game.Description,
                ReleaseDate = game.ReleaseDate,
                Price = game.Price,
                Publisher = game.Publisher,
                Genres = game.Genres,
                Platforms = game.Platforms,

                Reviews = reviewList,
                ReviewCount = reviewList.Count,
                AverageRating = averageRating,

                PartialData = reviews is null,
                Warnings = warnings.ToArray(),
                ResponseTimestamp = DateTime.UtcNow,
                Summary = $"Game '{game.Title}' — {reviewList.Count} reviews, avg rating: {averageRating?.ToString("0.0") ?? "N/A"}"
            };

            _logger.LogInformation("Successfully aggregated data for game {GameId}", gameId);
            return dto;
        }
    }
}