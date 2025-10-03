using FluentValidation;
using GameNest.ReviewsService.Application.Commands.CommentCommands.DeleteReply;

namespace GameNest.ReviewsService.Application.Validators.Comments
{
    public class DeleteReplyCommandValidator : AbstractValidator<DeleteReplyCommand>
    {
        public DeleteReplyCommandValidator()
        {
            RuleFor(x => x.CommentId)
                .NotEmpty();

            RuleFor(x => x.ReplyId)
                .NotEmpty();
        }
    }
}