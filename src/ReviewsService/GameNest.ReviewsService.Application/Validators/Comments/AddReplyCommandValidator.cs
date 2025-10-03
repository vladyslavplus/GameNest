using FluentValidation;
using GameNest.ReviewsService.Application.Commands.CommentCommands.AddReply;

namespace GameNest.ReviewsService.Application.Validators.Comments
{
    public class AddReplyCommandValidator : AbstractValidator<AddReplyCommand>
    {
        public AddReplyCommandValidator()
        {
            RuleFor(x => x.Reply)
                .NotNull();
        }
    }
}