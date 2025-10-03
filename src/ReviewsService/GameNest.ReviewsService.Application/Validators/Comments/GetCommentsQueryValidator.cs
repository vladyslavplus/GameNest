using FluentValidation;
using GameNest.ReviewsService.Application.Queries.CommentQueries.GetComments;

namespace GameNest.ReviewsService.Application.Validators.Comments
{
    public class GetCommentsQueryValidator : AbstractValidator<GetCommentsQuery>
    {
        public GetCommentsQueryValidator()
        {
            RuleFor(x => x.Parameters)
                .NotNull();

            RuleFor(x => x.Parameters.PageNumber)
                .GreaterThan(0);

            RuleFor(x => x.Parameters.PageSize)
                .InclusiveBetween(1, 100);

            When(x => x.Parameters.ReviewId != null, () =>
            {
                RuleFor(x => x.Parameters.ReviewId)
                    .NotEmpty()
                    .WithMessage("ReviewId must not be empty when provided.");
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