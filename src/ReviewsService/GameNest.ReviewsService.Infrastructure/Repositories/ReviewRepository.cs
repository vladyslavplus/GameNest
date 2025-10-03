using GameNest.ReviewsService.Domain.Common.Helpers;
using GameNest.ReviewsService.Domain.Entities;
using GameNest.ReviewsService.Domain.Entities.Parameters;
using GameNest.ReviewsService.Domain.Interfaces.Repositories;
using GameNest.ReviewsService.Infrastructure.Mongo.Context;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GameNest.ReviewsService.Infrastructure.Repositories
{
    public class ReviewRepository : MongoRepository<Review>, IReviewRepository
    {
        public ReviewRepository(MongoDbContext context, IClientSessionHandle? session = null)
            : base(context, session)
        {
        }

        public async Task<PagedList<Review>> GetReviewsAsync(ReviewParameters parameters, CancellationToken cancellationToken = default)
        {
            var filterBuilder = Builders<Review>.Filter;
            var filter = FilterDefinition<Review>.Empty;

            if (!string.IsNullOrWhiteSpace(parameters.GameId))
                filter &= filterBuilder.Eq(r => r.GameId, parameters.GameId);

            if (!string.IsNullOrWhiteSpace(parameters.CustomerId))
                filter &= filterBuilder.Eq(r => r.CustomerId, parameters.CustomerId);

            if (!string.IsNullOrWhiteSpace(parameters.SearchText))
                filter &= filterBuilder.Regex(r => r.Text.Value, new BsonRegularExpression(parameters.SearchText, "i"));

            var sortHelper = new MongoSortHelper<Review>();
            var findFluent = _collection.Find(filter)
                                        .Sort(sortHelper.ApplySort(parameters.OrderBy));

            return await PagedList<Review>
                .ToPagedListAsync(findFluent, parameters.PageNumber, parameters.PageSize, cancellationToken);
        }

        public async Task AddReplyAsync(string reviewId, Reply reply, CancellationToken cancellationToken = default)
        {
            var update = Builders<Review>.Update.Push(r => r.Replies, reply);
            await _collection.UpdateOneAsync(r => r.Id == reviewId, update, cancellationToken: cancellationToken);
        }

        public async Task UpdateReplyAsync(string reviewId, Reply reply, CancellationToken cancellationToken = default)
        {
            var filter = Builders<Review>.Filter.And(
                Builders<Review>.Filter.Eq(r => r.Id, reviewId),
                Builders<Review>.Filter.ElemMatch(r => r.Replies, rep => rep.Id == reply.Id)
            );

            var update = Builders<Review>.Update.Set(r => r.Replies[-1], reply);
            await _collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        }

        public async Task DeleteReplyAsync(string reviewId, string replyId, CancellationToken cancellationToken = default)
        {
            var update = Builders<Review>.Update.PullFilter(r => r.Replies, rep => rep.Id == replyId);
            await _collection.UpdateOneAsync(r => r.Id == reviewId, update, cancellationToken: cancellationToken);
        }
    }
}