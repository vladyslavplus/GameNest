﻿using GameNest.OrderService.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace GameNest.OrderService.Api.Middlewares
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode status;
            string title;
            string detail = exception.Message;

            switch (exception)
            {
                case NotFoundException:
                    status = HttpStatusCode.NotFound;
                    title = "Resource Not Found";
                    break;
                case BusinessConflictException:
                    status = HttpStatusCode.Conflict; 
                    title = "Business Conflict";
                    break;
                case ValidationException:
                    status = HttpStatusCode.BadRequest; 
                    title = "Validation Error";
                    break;
                default:
                    status = HttpStatusCode.InternalServerError; 
                    title = "Internal Server Error";
                    _logger.LogError(exception, "Unhandled exception occurred.");
                    break;
            }

            var problemDetails = new ProblemDetails
            {
                Status = (int)status,
                Title = title,
                Detail = detail,
                Instance = context.Request.Path
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, options));
        }
    }
}