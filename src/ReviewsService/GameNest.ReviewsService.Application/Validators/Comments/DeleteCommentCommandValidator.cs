using FluentValidation;
using GameNest.ReviewsService.Application.Commands.CommentCommands.DeleteComment;

namespace GameNest.ReviewsService.Application.Validators.Comments
{
    public class DeleteCommentCommandValidator : AbstractValidator<DeleteCommentCommand>
    {
        public DeleteCommentCommandValidator()
        {
            RuleFor(x => x.CommentId)
                .NotEmpty();
        }
    }
}