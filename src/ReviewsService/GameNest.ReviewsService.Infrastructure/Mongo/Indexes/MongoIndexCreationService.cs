using GameNest.ReviewsService.Domain.Entities;
using GameNest.ReviewsService.Infrastructure.Mongo.Context;
using MongoDB.Driver;

namespace GameNest.ReviewsService.Infrastructure.Mongo.Indexes
{
    public class MongoIndexCreationService : IIndexCreationService
    {
        private readonly MongoDbContext _context;

        public MongoIndexCreationService(MongoDbContext context)
        {
            _context = context;
        }

        public async Task CreateIndexesAsync(CancellationToken cancellationToken = default)
        {
            await _context.Reviews.Indexes.CreateOneAsync(
                new CreateIndexModel<Review>(Builders<Review>.IndexKeys.Ascending(r => r.GameId)),
                cancellationToken: cancellationToken
            );

            await _context.Reviews.Indexes.CreateOneAsync(
                new CreateIndexModel<Review>(Builders<Review>.IndexKeys.Ascending(r => r.CustomerId)),
                cancellationToken: cancellationToken
            );

            await _context.Reviews.Indexes.CreateOneAsync(
                new CreateIndexModel<Review>(
                    Builders<Review>.IndexKeys.Ascending(r => r.GameId).Ascending(r => r.CustomerId),
                    new CreateIndexOptions { Unique = true }
                ),
                cancellationToken: cancellationToken
            );

            await _context.Reviews.Indexes.CreateOneAsync(
                new CreateIndexModel<Review>(Builders<Review>.IndexKeys.Text(r => r.Text.Value)),
                cancellationToken: cancellationToken
            );

            await _context.Reviews.Indexes.CreateOneAsync(
                new CreateIndexModel<Review>(Builders<Review>.IndexKeys.Ascending(r => r.Rating.Value)),
                cancellationToken: cancellationToken
            );

            await _context.Comments.Indexes.CreateOneAsync(
                new CreateIndexModel<Comment>(
                    Builders<Comment>.IndexKeys
                        .Ascending(c => c.ReviewId)   
                        .Descending(c => c.CreatedAt) 
                        .Ascending(c => c.Id)
                ),
                cancellationToken: cancellationToken
            );

            await _context.Comments.Indexes.CreateOneAsync(
                new CreateIndexModel<Comment>(Builders<Comment>.IndexKeys.Ascending(c => c.ReviewId)),
                cancellationToken: cancellationToken
            );

            await _context.Comments.Indexes.CreateOneAsync(
                new CreateIndexModel<Comment>(Builders<Comment>.IndexKeys.Ascending(c => c.CustomerId)),
                cancellationToken: cancellationToken
            );

            await _context.Comments.Indexes.CreateOneAsync(
                new CreateIndexModel<Comment>(Builders<Comment>.IndexKeys.Text(c => c.Text.Value)),
                cancellationToken: cancellationToken
            );

            await _context.Media.Indexes.CreateOneAsync(
                new CreateIndexModel<Media>(Builders<Media>.IndexKeys.Ascending(m => m.GameId)),
                cancellationToken: cancellationToken
            );

            await _context.Media.Indexes.CreateOneAsync(
                new CreateIndexModel<Media>(Builders<Media>.IndexKeys.Ascending(m => m.CustomerId)),
                cancellationToken: cancellationToken
            );
        }
    }
}