using GameNest.ServiceDefaults.Metrics;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace GameNest.ServiceDefaults.Memory
{
    public class MemoryCacheService : IMemoryCacheService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<MemoryCacheService> _logger;

        private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(5);

        public MemoryCacheService(IMemoryCache cache, ILogger<MemoryCacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public T? Get<T>(string key)
        {
            var sw = Stopwatch.StartNew();

            if (_cache.TryGetValue(key, out T? value))
            {
                CacheMetrics.MemoryCacheHits.Add(1);
                CacheMetrics.CacheLatency.Record(sw.Elapsed.TotalSeconds);
                _logger.LogDebug("MemoryCache hit for key: {Key}", key);
                return value;
            }

            CacheMetrics.MemoryCacheMisses.Add(1);
            CacheMetrics.CacheLatency.Record(sw.Elapsed.TotalSeconds);
            _logger.LogDebug("MemoryCache miss for key: {Key}", key);
            return default;
        }

        public void Set<T>(string key, T value, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null)
        {
            var sw = Stopwatch.StartNew();

            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpiration ?? DefaultExpiration,
                SlidingExpiration = slidingExpiration,
                Priority = CacheItemPriority.Normal,
                Size = 1
            }.RegisterPostEvictionCallback((key, value, reason, state) =>
            {
                CacheMetrics.CacheEvictions.Add(1);
            });

            _cache.Set(key, value, options);
            sw.Stop();

            CacheMetrics.CacheLatency.Record(sw.Elapsed.TotalSeconds);
            CacheMetrics.UpdateMemoryCacheSize((_cache as MemoryCache)?.Count ?? 0);
        }

        public void Remove(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return;

            _cache.Remove(key);
            CacheMetrics.UpdateMemoryCacheSize((_cache as MemoryCache)?.Count ?? 0);
            _logger.LogDebug("MemoryCache removed key: {Key}", key);
        }

        public void Clear()
        {
            if (_cache is MemoryCache memCache)
            {
                memCache.Compact(1.0);
                CacheMetrics.CacheEvictions.Add(1);
                CacheMetrics.UpdateMemoryCacheSize(0);
                _logger.LogInformation("MemoryCache cleared");
            }
        }
    }
}