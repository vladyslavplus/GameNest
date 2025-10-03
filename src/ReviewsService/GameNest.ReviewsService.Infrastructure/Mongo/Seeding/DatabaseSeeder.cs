using GameNest.ReviewsService.Domain.Entities;
using GameNest.ReviewsService.Domain.ValueObjects;
using GameNest.ReviewsService.Infrastructure.Mongo.Context;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace GameNest.ReviewsService.Infrastructure.Mongo.Seeding
{
    public class DatabaseSeeder : IDataSeeder
    {
        private readonly MongoDbContext _context;
        private readonly ILogger<DatabaseSeeder> _logger;

        private static readonly List<(string GameId, string CustomerId, double Rating, string Text)> SeedReviews = new()
        {
            ("11111111-1111-1111-1111-111111111111", "22222222-2222-2222-2222-222222222222", 4.5, "Amazing game! Highly recommend."),
            ("33333333-3333-3333-3333-333333333333", "44444444-4444-4444-4444-444444444444", 3.8, "Good, but could be better."),
            ("55555555-5555-5555-5555-555555555555", "66666666-6666-6666-6666-666666666666", 5.0, "Perfect game! Best I've ever played."),
            ("77777777-7777-7777-7777-777777777777", "88888888-8888-8888-8888-888888888888", 2.5, "Not worth the money. Too many bugs.")
        };

        public DatabaseSeeder(MongoDbContext context, ILogger<DatabaseSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SeedAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting database seeding process...");

            try
            {
                var reviews = await SeedReviewsAsync(cancellationToken);
                await SeedCommentsAsync(reviews, cancellationToken);
                await SeedMediaAsync(reviews, cancellationToken);

                _logger.LogInformation("Database seeding completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during database seeding");
                throw;
            }
        }

        private async Task<List<Review>> SeedReviewsAsync(CancellationToken cancellationToken)
        {
            var reviewCount = await _context.Reviews.CountDocumentsAsync(
                FilterDefinition<Review>.Empty,
                cancellationToken: cancellationToken);

            List<Review> reviews;

            if (reviewCount == 0)
            {
                _logger.LogInformation("Seeding {Count} reviews...", SeedReviews.Count);

                reviews = SeedReviews.Select(r => new Review(
                    gameId: r.GameId,
                    customerId: r.CustomerId,
                    rating: new Rating(r.Rating),
                    text: new ReviewText(r.Text),
                    createdBy: "DatabaseSeeder"
                )).ToList();

                try
                {
                    await _context.Reviews.InsertManyAsync(reviews, cancellationToken: cancellationToken);
                    _logger.LogInformation("Successfully seeded {Count} reviews", reviews.Count);
                }
                catch (MongoBulkWriteException ex) when (ex.WriteErrors.Any(e => e.Code == 11000))
                {
                    _logger.LogWarning(ex, "Some reviews already exist due to unique constraint violations");
                    reviews = await _context.Reviews.Find(FilterDefinition<Review>.Empty)
                                                   .ToListAsync(cancellationToken);
                }
            }
            else
            {
                _logger.LogInformation("Reviews already exist ({Count} found), skipping seeding", reviewCount);
                reviews = await _context.Reviews.Find(FilterDefinition<Review>.Empty)
                                               .ToListAsync(cancellationToken);
            }

            return reviews;
        }

        private async Task SeedCommentsAsync(List<Review> reviews, CancellationToken cancellationToken)
        {
            var commentCount = await _context.Comments.CountDocumentsAsync(
                FilterDefinition<Comment>.Empty,
                cancellationToken: cancellationToken);

            if (commentCount == 0)
            {
                _logger.LogInformation("Seeding comments for {ReviewCount} reviews...", reviews.Count);

                var comments = new List<Comment>();
                var commentTexts = new[]
                {
                    "I totally agree with this review!",
                    "I had a different experience.",
                    "Thanks for sharing your opinion!",
                    "Very helpful review, thank you.",
                    "I'm thinking about buying this game too."
                };

                foreach (var review in reviews)
                {
                    var commentsToAdd = Random.Shared.Next(2, 4);
                    for (int i = 0; i < commentsToAdd; i++)
                    {
                        comments.Add(new Comment(
                            reviewId: review.Id,
                            customerId: i == 0 ? review.CustomerId : Guid.NewGuid().ToString(), 
                            text: new ReviewText(commentTexts[Random.Shared.Next(commentTexts.Length)]),
                            createdBy: "DatabaseSeeder"
                        ));
                    }
                }

                await _context.Comments.InsertManyAsync(comments, cancellationToken: cancellationToken);
                _logger.LogInformation("Successfully seeded {Count} comments", comments.Count);
            }
            else
            {
                _logger.LogInformation("Comments already exist ({Count} found), skipping seeding", commentCount);
            }
        }

        private async Task SeedMediaAsync(List<Review> reviews, CancellationToken cancellationToken)
        {
            var mediaCount = await _context.Media.CountDocumentsAsync(
                FilterDefinition<Media>.Empty,
                cancellationToken: cancellationToken);

            if (mediaCount == 0)
            {
                _logger.LogInformation("Seeding media for {ReviewCount} reviews...", reviews.Count);

                var mediaItems = new List<Media>();
                var mediaUrls = new[]
                {
                    "https://example.com/screenshot1.png",
                    "https://example.com/screenshot2.jpg",
                    "https://example.com/gameplay.mp4",
                    "https://example.com/review_image.png",
                    "https://example.com/game_photo.jpg"
                };

                foreach (var review in reviews)
                {
                    var mediaToAdd = Random.Shared.Next(1, 3);
                    for (int i = 0; i < mediaToAdd; i++)
                    {
                        mediaItems.Add(new Media(
                            gameId: review.GameId,
                            customerId: i == 0 ? review.CustomerId : Guid.NewGuid().ToString(),
                            url: new MediaUrl(mediaUrls[Random.Shared.Next(mediaUrls.Length)]),
                            createdBy: "DatabaseSeeder"
                        ));
                    }
                }

                await _context.Media.InsertManyAsync(mediaItems, cancellationToken: cancellationToken);
                _logger.LogInformation("Successfully seeded {Count} media items", mediaItems.Count);
            }
            else
            {
                _logger.LogInformation("Media already exists ({Count} found), skipping seeding", mediaCount);
            }
        }
    }
}