using GameNest.ReviewsService.Domain.Entities;
using GameNest.ReviewsService.Infrastructure.Mongo.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace GameNest.ReviewsService.Infrastructure.Mongo.Context
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;
        public IMongoDatabase Database => _database;
        public MongoDbContext(IOptions<MongoDbSettings> settings)
        {
            var mongoSettings = MongoClientSettings.FromConnectionString(settings.Value.ConnectionString);
            mongoSettings.MaxConnectionPoolSize = settings.Value.MaxConnectionPoolSize;
            mongoSettings.MinConnectionPoolSize = settings.Value.MinConnectionPoolSize;
            mongoSettings.ConnectTimeout = TimeSpan.FromSeconds(settings.Value.ConnectTimeoutSeconds);
            mongoSettings.SocketTimeout = TimeSpan.FromSeconds(settings.Value.SocketTimeoutSeconds);

            var client = new MongoClient(mongoSettings);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<Comment> Comments => _database.GetCollection<Comment>("comments");
        public IMongoCollection<Media> Media => _database.GetCollection<Media>("media");
        public IMongoCollection<Review> Reviews => _database.GetCollection<Review>("reviews");
        public IClientSessionHandle StartSession() => _database.Client.StartSession();
    }
}