using FluentValidation;
using GameNest.ReviewsService.Application.Commands.ReviewCommands.DeleteReplyFromReview;

namespace GameNest.ReviewsService.Application.Validators.Reviews
{
    public class DeleteReplyFromReviewCommandValidator : AbstractValidator<DeleteReplyFromReviewCommand>
    {
        public DeleteReplyFromReviewCommandValidator()
        {
            RuleFor(x => x.ReviewId)
                .NotEmpty();

            RuleFor(x => x.ReplyId)
                .NotEmpty();
        }
    }
}