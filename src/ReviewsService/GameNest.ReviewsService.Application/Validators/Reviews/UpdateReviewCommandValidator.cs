using FluentValidation;
using GameNest.ReviewsService.Application.Commands.ReviewCommands.UpdateReview;

namespace GameNest.ReviewsService.Application.Validators.Reviews
{
    public class UpdateReviewCommandValidator : AbstractValidator<UpdateReviewCommand>
    {
        public UpdateReviewCommandValidator()
        {
            RuleFor(x => x)
                .Must(x => x.NewText != null || x.NewRating != null)
                .WithMessage("Either NewText or NewRating must be provided.");

            When(x => x.NewText != null, () =>
            {
                RuleFor(x => x.NewText!.Value)
                    .NotEmpty()
                    .MaximumLength(1000);
            });

            When(x => x.NewRating != null, () =>
            {
                RuleFor(x => x.NewRating!.Value)
                    .InclusiveBetween(1, 5);
            });
        }
    }
}