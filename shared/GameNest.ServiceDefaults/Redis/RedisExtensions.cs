using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace GameNest.ServiceDefaults.Redis
{
    public static class RedisExtensions
    {
        public static IServiceCollection AddRedisCache(
            this IServiceCollection services,
            IConfiguration config)
        {
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var redisConnection =
                    config.GetConnectionString("redis") ??
                    config.GetConnectionString("Redis") ??
                    config["REDIS_CONNECTIONSTRING"] ??
                    Environment.GetEnvironmentVariable("REDIS_CONNECTIONSTRING") ??
                    throw new InvalidOperationException(
                        "Redis connection string not found. Expected 'ConnectionStrings:redis' or 'REDIS_CONNECTIONSTRING'.");

                var configuration = ConfigurationOptions.Parse(redisConnection);
                configuration.AbortOnConnectFail = false;
                configuration.ConnectRetry = 3;
                configuration.ConnectTimeout = 5000;
                configuration.SyncTimeout = 5000;

                return ConnectionMultiplexer.Connect(configuration);
            });

            services.AddSingleton<IRedisCacheService, RedisCacheService>();

            return services;
        }

        public static IServiceCollection AddDistributedRedisCache(
            this IServiceCollection services,
            IConfiguration config,
            string instanceName)
        {
            var redisConnection = config.GetConnectionString("redis")
                               ?? config.GetConnectionString("Redis")
                               ?? throw new InvalidOperationException(
                                   "Redis connection string not found. Expected 'redis' or 'Redis' in ConnectionStrings.");

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnection;
                options.InstanceName = $"{instanceName}:";
            });

            return services;
        }
    }
}