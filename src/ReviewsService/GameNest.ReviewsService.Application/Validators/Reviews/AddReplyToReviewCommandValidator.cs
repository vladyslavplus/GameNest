using FluentValidation;
using GameNest.ReviewsService.Application.Commands.ReviewCommands.AddReplyToReview;

namespace GameNest.ReviewsService.Application.Validators.Reviews
{
    public class AddReplyToReviewCommandValidator : AbstractValidator<AddReplyToReviewCommand>
    {
        public AddReplyToReviewCommandValidator()
        {
            RuleFor(x => x.Reply)
                .NotNull();
        }
    }
}