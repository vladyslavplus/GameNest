using Serilog;

namespace GameNest.OrderService.Api.Extensions
{
    public static class SerilogExtensions
    {
        public static IHostBuilder UseSerilogWithConfiguration(this IHostBuilder hostBuilder)
        {
            hostBuilder.UseSerilog((context, services, configuration) =>
            {
                configuration
                    .ReadFrom.Configuration(context.Configuration)   
                    .ReadFrom.Services(services)                     
                    .Enrich.FromLogContext();
            });

            return hostBuilder;
        }
    }
}
