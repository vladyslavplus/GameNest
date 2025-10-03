using FluentValidation;
using GameNest.ReviewsService.Application.Queries.ReviewQueries.GetReviews;

namespace GameNest.ReviewsService.Application.Validators.Reviews
{
    public class GetReviewsQueryValidator : AbstractValidator<GetReviewsQuery>
    {
        public GetReviewsQueryValidator()
        {
            RuleFor(x => x.Parameters)
                .NotNull();

            RuleFor(x => x.Parameters.PageNumber)
                .GreaterThan(0);

            RuleFor(x => x.Parameters.PageSize)
                .InclusiveBetween(1, 100);

            When(x => x.Parameters.GameId != null, () =>
            {
                RuleFor(x => x.Parameters.GameId)
                    .NotEmpty()
                    .WithMessage("GameId must not be empty when provided.");
            });

            When(x => x.Parameters.CustomerId != null, () =>
            {
                RuleFor(x => x.Parameters.CustomerId)
                    .NotEmpty()
                    .WithMessage("CustomerId must not be empty when provided.");
            });
        }
    }
}