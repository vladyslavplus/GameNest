using System.Diagnostics.Metrics;

namespace GameNest.ServiceDefaults.Metrics
{
    public static class CacheMetrics
    {
        private static readonly Meter Meter = new("GameNest.Cache", "1.0.0");

        public static readonly Counter<long> MemoryCacheHits = Meter.CreateCounter<long>("cache_memory_hit_total", "hits", "Number of L1 memory cache hits");
        public static readonly Counter<long> MemoryCacheMisses = Meter.CreateCounter<long>("cache_memory_miss_total", "misses", "Number of L1 memory cache misses");
        public static readonly Counter<long> RedisCacheHits = Meter.CreateCounter<long>("cache_redis_hit_total", "hits", "Number of L2 Redis cache hits");
        public static readonly Counter<long> RedisCacheMisses = Meter.CreateCounter<long>("cache_redis_miss_total", "misses", "Number of L2 Redis cache misses");

        public static readonly Histogram<double> CacheLatency = Meter.CreateHistogram<double>("cache_latency_seconds", "s", "Cache operation latency in seconds");

        public static readonly ObservableGauge<long> MemoryCacheSize =
            Meter.CreateObservableGauge("cache_memory_size_items", () => new Measurement<long>(GetMemoryCacheSize()));

        public static readonly Counter<long> CacheEvictions = Meter.CreateCounter<long>("cache_evictions_total", "evictions", "Number of L1 cache evictions");
        public static readonly Counter<long> CacheInvalidations = Meter.CreateCounter<long>("cache_invalidation_total", "events", "Number of cache invalidation broadcasts");

        private static long GetMemoryCacheSize()
        {
            return _memoryCacheSize;
        }

        private static long _memoryCacheSize = 0;
        public static void UpdateMemoryCacheSize(long newSize) => _memoryCacheSize = newSize;
    }
}