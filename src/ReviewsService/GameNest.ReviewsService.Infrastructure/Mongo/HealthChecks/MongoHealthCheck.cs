using GameNest.ReviewsService.Infrastructure.Mongo.Context;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GameNest.ReviewsService.Infrastructure.Mongo.HealthChecks
{
    public class MongoHealthCheck : IHealthCheck
    {
        private readonly MongoDbContext _context;

        public MongoHealthCheck(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var pingCommand = new BsonDocumentCommand<BsonDocument>(new BsonDocument("ping", 1));
                await _context.Reviews.Database.RunCommandAsync(pingCommand, cancellationToken: cancellationToken);
                return HealthCheckResult.Healthy("MongoDB is healthy.");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("MongoDB is unreachable.", ex);
            }
        }
    }
}