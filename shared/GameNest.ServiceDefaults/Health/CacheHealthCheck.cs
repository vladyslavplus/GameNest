using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace GameNest.ServiceDefaults.Health
{
    public class CacheHealthCheck : IHealthCheck
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IConnectionMultiplexer _redisMultiplexer;

        public CacheHealthCheck(IMemoryCache memoryCache, IConnectionMultiplexer redisMultiplexer)
        {
            _memoryCache = memoryCache;
            _redisMultiplexer = redisMultiplexer;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            var data = new Dictionary<string, object>();

            var memoryStats = _memoryCache.GetCurrentStatistics();
            if (memoryStats is not null)
            {
                data["L1_MemoryCache_Hits"] = memoryStats.TotalHits;
                data["L1_MemoryCache_Misses"] = memoryStats.TotalMisses;
                data["L1_MemoryCache_CurrentEntries"] = memoryStats.CurrentEntryCount;
                data["L1_MemoryCache_CurrentSize"] = memoryStats.CurrentEstimatedSize!;
            }

            try
            {
                var redisDb = _redisMultiplexer.GetDatabase();
                await redisDb.PingAsync();
                data["L2_Redis_Status"] = "Connected";
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Redis connection failed.", ex, data);
            }

            return HealthCheckResult.Healthy("Cache services are running.", data);
        }
    }
}