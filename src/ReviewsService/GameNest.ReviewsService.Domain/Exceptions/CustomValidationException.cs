using FluentValidation;
using FluentValidation.Results;

namespace GameNest.ReviewsService.Domain.Exceptions
{
    public class CustomValidationException : ValidationException
    {
        public CustomValidationException(IEnumerable<ValidationFailure> failures)
            : base(failures) { }
    }
}