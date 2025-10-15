using GameNest.ServiceDefaults.Background.Cache;
using Microsoft.Extensions.DependencyInjection;

namespace GameNest.ServiceDefaults.Extensions
{
    public static class BackgroundJobsExtensions
    {
        public static IServiceCollection AddCacheBackgroundJobs(this IServiceCollection services)
        {
            services.AddHostedService<CacheRefreshBackgroundService>();
            return services;
        }
    }
}