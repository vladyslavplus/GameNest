using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace GameNest.ReviewsService.Application.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var requestGuid = Guid.NewGuid().ToString();

            _logger.LogInformation(
                "[START] {RequestName} ({RequestId}) - Request: {Request}",
                requestName, requestGuid, JsonSerializer.Serialize(request));

            var response = await next();

            _logger.LogInformation(
                "[SUCCESS] {RequestName} ({RequestId}) completed successfully",
                requestName, requestGuid);

            return response;
        }
    }
}