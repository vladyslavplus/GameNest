using FluentValidation;
using GameNest.ReviewsService.Application.Commands.CommentCommands.UpdateCommentText;

namespace GameNest.ReviewsService.Application.Validators.Comments
{
    public class UpdateCommentTextCommandValidator : AbstractValidator<UpdateCommentTextCommand>
    {
        public UpdateCommentTextCommandValidator()
        {
            RuleFor(x => x.NewText)
                .NotNull()
                .Must(t => !string.IsNullOrWhiteSpace(t.Value))
                .WithMessage("New text cannot be empty.")
                .Must(t => t.Value.Length <= 1000)
                .WithMessage("New text cannot exceed 1000 characters.");
        }
    }
}