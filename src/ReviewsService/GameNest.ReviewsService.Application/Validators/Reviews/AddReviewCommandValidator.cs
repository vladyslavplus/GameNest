using FluentValidation;
using GameNest.ReviewsService.Application.Commands.ReviewCommands.AddReview;

namespace GameNest.ReviewsService.Application.Validators.Reviews
{
    public class AddReviewCommandValidator : AbstractValidator<AddReviewCommand>
    {
        public AddReviewCommandValidator()
        {
            RuleFor(x => x.GameId)
                .NotEmpty();

            RuleFor(x => x.CustomerId)
                .NotEmpty();

            RuleFor(x => x.Rating)
                .NotNull()
                .Must(r => r.Value >= 1 && r.Value <= 5)
                .WithMessage("Rating must be between 1 and 5.");

            RuleFor(x => x.Text)
                .NotNull()
                .Must(t => !string.IsNullOrWhiteSpace(t.Value))
                .WithMessage("Review text cannot be empty.");
        }
    }
}