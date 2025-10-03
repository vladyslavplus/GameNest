using GameNest.ReviewsService.Domain.Common.Helpers;
using GameNest.ReviewsService.Domain.Entities;
using GameNest.ReviewsService.Domain.Entities.Parameters;
using GameNest.ReviewsService.Domain.Interfaces.Repositories;
using GameNest.ReviewsService.Infrastructure.Mongo.Context;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GameNest.ReviewsService.Infrastructure.Repositories
{
    public class MediaRepository : MongoRepository<Media>, IMediaRepository
    {
        public MediaRepository(MongoDbContext context, IClientSessionHandle? session = null)
            : base(context, session)
        {
        }

        public async Task<PagedList<Media>> GetMediaAsync(MediaParameters parameters, CancellationToken cancellationToken = default)
        {
            var filterBuilder = Builders<Media>.Filter;
            var filter = FilterDefinition<Media>.Empty;

            if (!string.IsNullOrWhiteSpace(parameters.GameId))
                filter &= filterBuilder.Eq(m => m.GameId, parameters.GameId);

            if (!string.IsNullOrWhiteSpace(parameters.CustomerId))
                filter &= filterBuilder.Eq(m => m.CustomerId, parameters.CustomerId);

            if (!string.IsNullOrWhiteSpace(parameters.UrlContains))
                filter &= filterBuilder.Regex(m => m.Url.Value, new BsonRegularExpression(parameters.UrlContains, "i"));

            var sortHelper = new MongoSortHelper<Media>();
            var findFluent = _collection.Find(filter)
                                        .Sort(sortHelper.ApplySort(parameters.OrderBy));

            return await PagedList<Media>
                .ToPagedListAsync(findFluent, parameters.PageNumber, parameters.PageSize, cancellationToken);
        }
    }
}