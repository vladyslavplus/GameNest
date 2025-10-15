using GameNest.AggregatorService.Clients;
using GameNest.AggregatorService.DTOs.Aggregated;
using GameNest.AggregatorService.DTOs.Catalog;
using GameNest.AggregatorService.DTOs.Reviews;
using Grpc.Core;
using System.Globalization;

namespace GameNest.AggregatorService.Services
{
    public class GameAggregatorService
    {
        private readonly CatalogGrpcClient _catalogClient;
        private readonly ReviewsGrpcClient _reviewsClient;
        private readonly ILogger<GameAggregatorService> _logger;

        public GameAggregatorService(
            CatalogGrpcClient catalogClient,
            ReviewsGrpcClient reviewsClient,
            ILogger<GameAggregatorService> logger)
        {
            _catalogClient = catalogClient;
            _reviewsClient = reviewsClient;
            _logger = logger;
        }

        public async Task<IEnumerable<AggregatedGameDto>?> GetAllAggregatedGamesAsync(CancellationToken ct)
        {
            try
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
                    var reviewList = new List<Grpc.Reviews.Review>();

                    try
                    {
                        var reviews = await _reviewsClient.GetReviewsByGameIdAsync(game.Id, ct);
                        reviewList = reviews?.ToList() ?? new List<Grpc.Reviews.Review>();

                        if (reviews is null)
                        {
                            warnings.Add($"Reviews service returned null for game '{game.Title}'.");
                            _logger.LogWarning("Reviews service returned null for GameId {GameId}", game.Id);
                        }
                    }
                    catch (RpcException ex)
                    {
                        warnings.Add($"Reviews service is unavailable for game '{game.Title}'. Error: {ex.Status.Detail}");
                        _logger.LogError(ex, "Failed to fetch reviews for GameId {GameId} due to gRPC error", game.Id);
                    }
                    catch (Exception ex)
                    {
                        warnings.Add($"Unexpected error fetching reviews for game '{game.Title}': {ex.Message}");
                        _logger.LogError(ex, "Unexpected error fetching reviews for GameId {GameId}", game.Id);
                    }

                    var averageRating = reviewList.Any()
                        ? reviewList.Average(r => r.Rating)
                        : (double?)null;

                    var dto = new AggregatedGameDto
                    {
                        Id = Guid.Parse(game.Id),
                        Title = game.Title,
                        Description = game.Description,
                        ReleaseDate = ParseDate(game.ReleaseDate),
                        Price = (decimal)game.Price,
                        Publisher = string.IsNullOrEmpty(game.PublisherName)
                            ? null
                            : new PublisherDto
                            {
                                Name = game.PublisherName
                            },
                        Genres = game.Genres.ToList(),
                        Platforms = game.Platforms.ToList(),

                        Reviews = reviewList.Select(MapReviewToDto).ToList(),
                        ReviewCount = reviewList.Count,
                        AverageRating = averageRating,

                        PartialData = warnings.Any(),
                        Warnings = warnings.ToArray(),
                        ResponseTimestamp = DateTime.UtcNow,
                        Summary = warnings.Any()
                            ? $"Game '{game.Title}' — Partial data (reviews unavailable)"
                            : $"Game '{game.Title}' — {reviewList.Count} reviews, avg rating: {averageRating?.ToString("0.0") ?? "N/A"}"
                    };

                    aggregatedGames.Add(dto);
                }

                _logger.LogInformation("Successfully aggregated data for {GameCount} games.", aggregatedGames.Count);
                return aggregatedGames;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch games from catalog service");
                throw;
            }
        }

        public async Task<AggregatedGameDto?> GetAggregatedGameByIdAsync(Guid gameId, CancellationToken ct)
        {
            try
            {
                var game = await _catalogClient.GetGameByIdAsync(gameId, ct);
                if (game is null)
                {
                    _logger.LogWarning("Game with ID {GameId} not found", gameId);
                    return null;
                }

                var warnings = new List<string>();
                var reviewList = new List<GameNest.Grpc.Reviews.Review>();

                try
                {
                    var reviews = await _reviewsClient.GetReviewsByGameIdAsync(game.Id, ct);
                    reviewList = reviews?.ToList() ?? new List<GameNest.Grpc.Reviews.Review>();

                    if (reviews is null)
                    {
                        warnings.Add($"Reviews service returned null for game '{game.Title}'.");
                        _logger.LogWarning("Reviews service returned null for GameId {GameId}", game.Id);
                    }
                }
                catch (RpcException ex)
                {
                    warnings.Add($"Reviews service is unavailable. Error: {ex.Status.Detail}");
                    _logger.LogError(ex, "Failed to fetch reviews for GameId {GameId} due to gRPC error", game.Id);
                }
                catch (Exception ex)
                {
                    warnings.Add($"Unexpected error fetching reviews: {ex.Message}");
                    _logger.LogError(ex, "Unexpected error fetching reviews for GameId {GameId}", game.Id);
                }

                var averageRating = reviewList.Any()
                    ? reviewList.Average(r => r.Rating)
                    : (double?)null;

                var dto = new AggregatedGameDto
                {
                    Id = Guid.Parse(game.Id),
                    Title = game.Title,
                    Description = game.Description,
                    ReleaseDate = ParseDate(game.ReleaseDate),
                    Price = (decimal)game.Price,
                    Publisher = string.IsNullOrEmpty(game.PublisherName)
                        ? null
                        : new PublisherDto
                        {
                            Name = game.PublisherName
                        },
                    Genres = game.Genres.ToList(),
                    Platforms = game.Platforms.ToList(),

                    Reviews = reviewList.Select(MapReviewToDto).ToList(),
                    ReviewCount = reviewList.Count,
                    AverageRating = averageRating,

                    PartialData = warnings.Any(),
                    Warnings = warnings.ToArray(),
                    ResponseTimestamp = DateTime.UtcNow,
                    Summary = warnings.Any()
                        ? $"Game '{game.Title}' — Partial data (reviews unavailable)"
                        : $"Game '{game.Title}' — {reviewList.Count} reviews, avg rating: {averageRating?.ToString("0.0") ?? "N/A"}"
                };

                _logger.LogInformation("Successfully aggregated data for game {GameId}", gameId);
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch game {GameId} from catalog service", gameId);
                throw;
            }
        }

        private static ReviewDto MapReviewToDto(Grpc.Reviews.Review review)
        {
            return new ReviewDto
            {
                Id = review.Id,
                GameId = review.GameId,
                CustomerId = review.CustomerId,
                Rating = new RatingDto { Value = review.Rating },
                Text = new TextDto
                {
                    Value = review.Text,
                    WordCount = review.Text?.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length ?? 0,
                    IsLongReview = (review.Text?.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length ?? 0) > 50
                },
                Replies = review.Replies.Select(r => new ReplyDto
                {
                    Id = r.Id,
                    CustomerId = r.CustomerId,
                    Text = r.Text
                }).ToList()
            };
        }

        private static DateTime? ParseDate(string? dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
                return null;

            if (DateTime.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                return date;

            return null;
        }
    }
}