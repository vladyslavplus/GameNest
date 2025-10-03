using GameNest.ReviewsService.Domain.Common;
using GameNest.ReviewsService.Infrastructure.Mongo.Context;
using MongoDB.Driver;

namespace GameNest.ReviewsService.Infrastructure.Repositories
{
    public class MongoRepository<T> : IMongoRepository<T> where T : BaseEntity
    {
        protected readonly IMongoCollection<T> _collection;
        protected readonly IClientSessionHandle? _session;

        public MongoRepository(MongoDbContext context, IClientSessionHandle? session = null)
        {
            _collection = typeof(T).Name switch
            {
                "Comment" => (IMongoCollection<T>)context.Comments,
                "Media" => (IMongoCollection<T>)context.Media,
                "Review" => (IMongoCollection<T>)context.Reviews,
                _ => context.Database.GetCollection<T>(typeof(T).Name + "s")
            };
            _session = session;
        }

        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            if (_session != null)
                await _collection.InsertOneAsync(_session, entity, cancellationToken: cancellationToken);
            else
                await _collection.InsertOneAsync(entity, cancellationToken: cancellationToken);

            return entity;
        }

        public async Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _collection.Find(e => e.Id == id).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _collection.ReplaceOneAsync(e => e.Id == entity.Id, entity, cancellationToken: cancellationToken);
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            await _collection.DeleteOneAsync(e => e.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<T>> ListAsync(CancellationToken cancellationToken = default)
        {
            return await _collection.Find(_ => true).ToListAsync(cancellationToken);
        }
    }
}