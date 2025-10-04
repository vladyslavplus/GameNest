using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Net;
using System.Text.Json;

namespace GameNest.CatalogService.Api.Middlewares
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _environment;

        public GlobalExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionHandlingMiddleware> logger,
            IHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred: {ExceptionType}", ex.GetType().Name);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var correlationId = context.Items["X-Correlation-Id"]?.ToString();

            var problemDetails = new ProblemDetails
            {
                Instance = context.Request.Path,
                Title = "An unexpected error occurred.",
                Status = (int)HttpStatusCode.InternalServerError,
                Detail = _environment.IsProduction()
                    ? "An error occurred while processing your request."
                    : exception.Message
            };

            if (!string.IsNullOrEmpty(correlationId))
            {
                problemDetails.Extensions["correlationId"] = correlationId;
            }

            problemDetails.Extensions["traceId"] = context.TraceIdentifier;

            switch (exception)
            {
                case KeyNotFoundException:
                    problemDetails.Title = "Resource not found.";
                    problemDetails.Status = (int)HttpStatusCode.NotFound;
                    _logger.LogWarning("Resource not found: {Message}", exception.Message);
                    break;

                case ValidationException validationEx:
                    problemDetails.Title = "Validation error.";
                    problemDetails.Status = (int)HttpStatusCode.BadRequest;
                    problemDetails.Extensions["errors"] = validationEx.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.ErrorMessage).ToArray()
                        );
                    _logger.LogWarning("Validation error: {Errors}",
                        string.Join(", ", validationEx.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}")));
                    break;

                case InvalidOperationException:
                    problemDetails.Title = "Conflict occurred.";
                    problemDetails.Status = (int)HttpStatusCode.Conflict;
                    _logger.LogWarning("Invalid operation: {Message}", exception.Message);
                    break;

                case DbUpdateException dbEx when dbEx.InnerException is PostgresException pgEx:
                    HandleDatabaseException(problemDetails, pgEx);
                    break;

                case UnauthorizedAccessException:
                    problemDetails.Title = "Unauthorized.";
                    problemDetails.Status = (int)HttpStatusCode.Unauthorized;
                    problemDetails.Detail = "You are not authorized to perform this action.";
                    _logger.LogWarning("Unauthorized access attempt: {Message}", exception.Message);
                    break;

                default:
                    problemDetails.Title = "Server error.";
                    problemDetails.Status = (int)HttpStatusCode.InternalServerError;
                    _logger.LogError(exception, "Critical unhandled exception");
                    break;
            }

            if (_environment.IsDevelopment() && exception.StackTrace != null)
            {
                problemDetails.Extensions["stackTrace"] = exception.StackTrace;
            }

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = problemDetails.Status ?? (int)HttpStatusCode.InternalServerError;

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = _environment.IsDevelopment()
            };

            var json = JsonSerializer.Serialize(problemDetails, options);
            await context.Response.WriteAsync(json);
        }

        private void HandleDatabaseException(ProblemDetails problemDetails, PostgresException pgEx)
        {
            switch (pgEx.SqlState)
            {
                case PostgresErrorCodes.UniqueViolation:
                    problemDetails.Title = "Duplicate key violation.";
                    problemDetails.Detail = "A record with this key already exists.";
                    problemDetails.Status = (int)HttpStatusCode.Conflict;
                    _logger.LogWarning("Database unique violation: {Message}", pgEx.MessageText);
                    break;

                case PostgresErrorCodes.ForeignKeyViolation:
                    problemDetails.Title = "Foreign key violation.";
                    problemDetails.Detail = "Referenced record does not exist.";
                    problemDetails.Status = (int)HttpStatusCode.BadRequest;
                    _logger.LogWarning("Database foreign key violation: {Message}", pgEx.MessageText);
                    break;

                case PostgresErrorCodes.NotNullViolation:
                    problemDetails.Title = "Required field missing.";
                    problemDetails.Detail = "A required field was not provided.";
                    problemDetails.Status = (int)HttpStatusCode.BadRequest;
                    _logger.LogWarning("Database not null violation: {Message}", pgEx.MessageText);
                    break;

                default:
                    problemDetails.Title = "Database error.";
                    problemDetails.Detail = _environment.IsProduction()
                        ? "A database error occurred."
                        : pgEx.MessageText;
                    problemDetails.Status = (int)HttpStatusCode.InternalServerError;
                    _logger.LogError(pgEx, "Database error: {SqlState}", pgEx.SqlState);
                    break;
            }
        }
    }
}