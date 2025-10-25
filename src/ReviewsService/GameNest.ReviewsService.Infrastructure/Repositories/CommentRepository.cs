using GameNest.ReviewsService.Domain.Common.Helpers;
using GameNest.ReviewsService.Domain.Entities;
using GameNest.ReviewsService.Domain.Entities.Parameters;
using GameNest.ReviewsService.Domain.Interfaces.Repositories;
using GameNest.ReviewsService.Infrastructure.Mongo.Context;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GameNest.ReviewsService.Infrastructure.Repositories
{
    public class CommentRepository : MongoRepository<Comment>, ICommentRepository
    {
        public CommentRepository(MongoDbContext context, IClientSessionHandle? session = null)
            : base(context, session)
        {
        }

        public async Task<PagedList<Comment>> GetCommentsAsync(CommentParameters parameters, CancellationToken cancellationToken = default)
        {
            var filterBuilder = Builders<Comment>.Filter;
            var filter = FilterDefinition<Comment>.Empty;

            if (!string.IsNullOrWhiteSpace(parameters.ReviewId))
                filter &= filterBuilder.Eq(c => c.ReviewId, parameters.ReviewId);

            if (!string.IsNullOrWhiteSpace(parameters.CustomerId))
                filter &= filterBuilder.Eq(c => c.CustomerId, parameters.CustomerId);

            if (!string.IsNullOrWhiteSpace(parameters.SearchText))
                filter &= filterBuilder.Regex(c => c.Text.Value, new BsonRegularExpression(parameters.SearchText, "i"));

            var sortHelper = new MongoSortHelper<Comment>();
            var sort = sortHelper.ApplySort(parameters.OrderBy ?? "Id asc");

            if (!string.IsNullOrWhiteSpace(parameters.CursorId))
            {
                var lastItem = await _collection.Find(Builders<Comment>.Filter.Eq(c => c.Id, parameters.CursorId))
                                                .FirstOrDefaultAsync(cancellationToken);

                if (lastItem != null)
                {
                    filter &= Builders<Comment>.Filter.Or(
                        Builders<Comment>.Filter.Lt(c => c.CreatedAt, lastItem.CreatedAt),
                        Builders<Comment>.Filter.And(
                            Builders<Comment>.Filter.Eq(c => c.CreatedAt, lastItem.CreatedAt),
                            Builders<Comment>.Filter.Gt(c => c.Id, lastItem.Id)
                        )
                    );
                }
            }

            var items = await _collection.Find(filter)
                                         .Sort(sort)
                                         .Limit(parameters.PageSize)
                                         .ToListAsync(cancellationToken);

            var pagedList = PagedList<Comment>.FromCursor(items, parameters.PageSize);
            return pagedList;
        }

        public async Task AddReplyAsync(string commentId, Reply reply, CancellationToken cancellationToken = default)
        {
            var update = Builders<Comment>.Update.Push(c => c.Replies, reply);
            await _collection.UpdateOneAsync(c => c.Id == commentId, update, cancellationToken: cancellationToken);
        }

        public async Task UpdateReplyAsync(string commentId, Reply reply, CancellationToken cancellationToken = default)
        {
            var filter = Builders<Comment>.Filter.And(
                Builders<Comment>.Filter.Eq(c => c.Id, commentId),
                Builders<Comment>.Filter.ElemMatch(c => c.Replies, r => r.Id == reply.Id)
            );

            var update = Builders<Comment>.Update.Set("Replies.$", reply);
            await _collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        }

        public async Task DeleteReplyAsync(string commentId, string replyId, CancellationToken cancellationToken = default)
        {
            var update = Builders<Comment>.Update.PullFilter(c => c.Replies, r => r.Id == replyId);
            await _collection.UpdateOneAsync(c => c.Id == commentId, update, cancellationToken: cancellationToken);
        }
    }
}