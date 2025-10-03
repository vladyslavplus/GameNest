using FluentValidation;
using GameNest.ReviewsService.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GameNest.ReviewsService.Application.Behaviors
{
    public class ExceptionHandlingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> _logger;

        public ExceptionHandlingBehavior(ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            try
            {
                return await next();
            }
            catch (DomainException dex)
            {
                _logger.LogWarning(dex,
                    "Domain validation failed in {RequestName}: {@Request}",
                    typeof(TRequest).Name, request);
                throw;
            }
            catch (ValidationException vex)
            {
                foreach (var failure in vex.Errors)
                {
                    _logger.LogWarning(
                        vex,
                        "Validation failure in {RequestName}: Property {PropertyName}, Error: {ErrorMessage}",
                        typeof(TRequest).Name, failure.PropertyName, failure.ErrorMessage);
                }

                throw; 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Unhandled exception in {RequestName}: {@Request}",
                    typeof(TRequest).Name, request);
                throw;
            }
        }
    }
}