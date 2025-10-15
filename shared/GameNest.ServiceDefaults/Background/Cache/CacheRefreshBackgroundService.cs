using GameNest.ServiceDefaults.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GameNest.ServiceDefaults.Background.Cache
{
    public class CacheRefreshBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CacheRefreshBackgroundService> _logger;
        private readonly TimeSpan _interval;

        public CacheRefreshBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<CacheRefreshBackgroundService> logger,
            IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _interval = TimeSpan.FromMinutes(configuration.GetValue<int>("CacheSettings:RefreshIntervalMinutes", 10));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Cache refresh background service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Starting cache refresh cycle.");

                using (var scope = _serviceProvider.CreateScope())
                {
                    var preloaders = scope.ServiceProvider.GetServices<ICachePreloader>();

                    var preloadTasks = preloaders.Select(async preloader =>
                    {
                        try
                        {
                            _logger.LogInformation("Running cache preloader: {Preloader}", preloader.GetType().Name);
                            await preloader.PreloadAsync(stoppingToken);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error during cache refresh for {Preloader}.", preloader.GetType().Name);
                        }
                    });

                    await Task.WhenAll(preloadTasks);
                }

                _logger.LogInformation("Cache refresh cycle completed. Waiting {Interval} before next run.", _interval);
                await Task.Delay(_interval, stoppingToken);
            }

            _logger.LogInformation("Cache refresh background service stopped.");
        }
    }
}