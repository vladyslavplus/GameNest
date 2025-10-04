using GameNest.ServiceDefaults.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace GameNest.ServiceDefaults.Extensions
{
    public static class CorrelationIdMiddlewareExtensions
    {
        public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
        {
            return app.UseMiddleware<CorrelationIdMiddleware>();
        }
    }
}