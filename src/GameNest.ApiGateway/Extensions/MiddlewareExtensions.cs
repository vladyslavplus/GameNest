using GameNest.ApiGateway.Middlewares;
using GameNest.ServiceDefaults.Extensions;

namespace GameNest.ApiGateway.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseGatewayPipeline(this IApplicationBuilder app)
        {
            app.UseCorrelationId();
            app.UseMiddleware<RateLimitingMiddleware>();
            app.UseMiddleware<GatewayLoggingMiddleware>();
            app.UseMiddleware<GatewayRequestMiddleware>();
            app.UseMiddleware<TimeoutMiddleware>();
            app.UseMiddleware<YarpProxyLoggingMiddleware>();
            return app;
        }
    }
}
