using FluentValidation;
using GameNest.ReviewsService.Application.Commands.ReviewCommands.DeleteReview;

namespace GameNest.ReviewsService.Application.Validators.Reviews
{
    public class DeleteReviewCommandValidator : AbstractValidator<DeleteReviewCommand>
    {
        public DeleteReviewCommandValidator()
        {
            RuleFor(x => x.ReviewId)
                .NotEmpty();
        }
    }
}