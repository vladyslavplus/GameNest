using GameNest.ReviewsService.Domain.Entities;
using GameNest.ReviewsService.Domain.ValueObjects;
using GameNest.ReviewsService.Infrastructure.Mongo.Context;
using GameNest.Shared.TestData;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace GameNest.ReviewsService.Infrastructure.Mongo.Seeding
{
    public class DatabaseSeeder : IDataSeeder
    {
        private readonly MongoDbContext _context;
        private readonly ILogger<DatabaseSeeder> _logger;

        public DatabaseSeeder(MongoDbContext context, ILogger<DatabaseSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SeedAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting database seeding for ReviewsService...");

            try
            {
                var reviews = await SeedReviewsAsync(cancellationToken);
                await SeedCommentsAsync(reviews, cancellationToken);
                await SeedMediaAsync(reviews, cancellationToken);

                _logger.LogInformation("Database seeding completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during database seeding.");
                throw;
            }
        }

        private async Task<List<Review>> SeedReviewsAsync(CancellationToken cancellationToken)
        {
            if (await _context.Reviews.CountDocumentsAsync(FilterDefinition<Review>.Empty, cancellationToken: cancellationToken) > 0)
            {
                _logger.LogInformation("Reviews already exist. Skipping seeding.");
                return await _context.Reviews.Find(FilterDefinition<Review>.Empty).ToListAsync(cancellationToken);
            }

            var reviewData = new List<(string GameId, string UserId, double Rating, string Text)>
            {
                (SharedSeedData.Games.TheWitcher3, SharedSeedData.Users.JohnDoe, 4.5, SharedSeedData.ReviewTexts.Default[0]),
                (SharedSeedData.Games.DoomEternal, SharedSeedData.Users.AliceWonder, 3.8, SharedSeedData.ReviewTexts.Default[1]),
                (SharedSeedData.Games.StardewValley, SharedSeedData.Users.MarkSmith, 5.0, SharedSeedData.ReviewTexts.Default[2]),
                (SharedSeedData.Games.Cyberpunk2077, SharedSeedData.Users.Admin, 2.5, SharedSeedData.ReviewTexts.Default[3])
            };

            var reviews = reviewData.Select(r => new Review(
                gameId: r.GameId,
                customerId: r.UserId,
                rating: new Rating(r.Rating),
                text: new ReviewText(r.Text),
                createdBy: "DatabaseSeeder"
            )).ToList();

            await _context.Reviews.InsertManyAsync(reviews, cancellationToken: cancellationToken);
            _logger.LogInformation("Seeded {Count} reviews.", reviews.Count);

            return reviews;
        }

        private async Task SeedCommentsAsync(List<Review> reviews, CancellationToken cancellationToken)
        {
            if (await _context.Comments.CountDocumentsAsync(FilterDefinition<Comment>.Empty, cancellationToken: cancellationToken) > 0)
            {
                _logger.LogInformation("Comments already exist. Skipping seeding.");
                return;
            }

            var comments = new List<Comment>();
            foreach (var review in reviews)
            {
                var count = Random.Shared.Next(2, 4);
                for (int i = 0; i < count; i++)
                {
                    comments.Add(new Comment(
                        reviewId: review.Id,
                        customerId: i == 0 ? review.CustomerId : Guid.NewGuid().ToString(),
                        text: new ReviewText(SharedSeedData.Comments.Default[Random.Shared.Next(SharedSeedData.Comments.Default.Count)]),
                        createdBy: "DatabaseSeeder"
                    ));
                }
            }

            await _context.Comments.InsertManyAsync(comments, cancellationToken: cancellationToken);
            _logger.LogInformation("Seeded {Count} comments.", comments.Count);
        }

        private async Task SeedMediaAsync(List<Review> reviews, CancellationToken cancellationToken)
        {
            if (await _context.Media.CountDocumentsAsync(FilterDefinition<Media>.Empty, cancellationToken: cancellationToken) > 0)
            {
                _logger.LogInformation("Media already exist. Skipping seeding.");
                return;
            }

            var media = new List<Media>();
            foreach (var review in reviews)
            {
                var count = Random.Shared.Next(1, 3);
                for (int i = 0; i < count; i++)
                {
                    media.Add(new Media(
                        gameId: review.GameId,
                        customerId: i == 0 ? review.CustomerId : Guid.NewGuid().ToString(),
                        url: new MediaUrl(SharedSeedData.Media.Urls[Random.Shared.Next(SharedSeedData.Media.Urls.Count)]),
                        createdBy: "DatabaseSeeder"
                    ));
                }
            }

            await _context.Media.InsertManyAsync(media, cancellationToken: cancellationToken);
            _logger.LogInformation("Seeded {Count} media items.", media.Count);
        }
    }
}
