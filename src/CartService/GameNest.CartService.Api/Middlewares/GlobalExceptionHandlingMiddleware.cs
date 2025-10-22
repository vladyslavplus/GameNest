using StackExchange.Redis;
using System.Net;
using System.Text.Json;

namespace GameNest.CartService.Api.Middlewares
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public GlobalExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionHandlingMiddleware> logger,
            IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred: {ErrorMessage}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/problem+json";
            HttpStatusCode statusCode;
            string title;
            string detail;

            switch (exception)
            {
                case RedisTimeoutException or RedisConnectionException:
                    statusCode = HttpStatusCode.ServiceUnavailable;
                    title = "Service Unavailable";
                    detail = "Our services are temporarily unavailable. Please try again in a few moments.";
                    break;

                case KeyNotFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    title = "Resource Not Found";
                    detail = "The requested resource was not found.";
                    break;

                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    title = "Internal Server Error";
                    detail = "An unexpected error occurred. Please contact support.";
                    break;
            }

            if (_env.IsDevelopment())
            {
                detail = exception.Message;
            }

            context.Response.StatusCode = (int)statusCode;

            var problemDetails = new
            {
                Status = (int)statusCode,
                Title = title,
                Detail = detail
            };

            var jsonResponse = JsonSerializer.Serialize(problemDetails);
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
