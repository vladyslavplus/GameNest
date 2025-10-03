using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace GameNest.ReviewsService.Application.Behaviors
{
    public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
        private readonly Stopwatch _timer;

        public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
            _timer = new Stopwatch();
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            _timer.Start();

            var response = await next();

            _timer.Stop();

            var elapsedMilliseconds = _timer.ElapsedMilliseconds;
            var requestName = typeof(TRequest).Name;

            if (elapsedMilliseconds > 500) 
            {
                _logger.LogWarning(
                    "[PERFORMANCE] {RequestName} took {ElapsedMilliseconds}ms to execute",
                    requestName, elapsedMilliseconds);
            }
            else
            {
                _logger.LogInformation(
                    "[PERFORMANCE] {RequestName} executed in {ElapsedMilliseconds}ms",
                    requestName, elapsedMilliseconds);
            }

            return response;
        }
    }
}