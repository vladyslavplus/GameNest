using Serilog.Context;

namespace GameNest.CatalogService.Api.Middlewares
{
    public class LoggingEnrichmentMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingEnrichmentMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId = context.TraceIdentifier;
            var userId = context.User?.Identity?.IsAuthenticated == true
                ? context.User.FindFirst("sub")?.Value 
                : "anonymous";

            using (LogContext.PushProperty("CorrelationId", correlationId))
            using (LogContext.PushProperty("UserId", userId))
            using (LogContext.PushProperty("RequestPath", context.Request.Path))
            {
                await _next(context);
            }
        }
    }
}
