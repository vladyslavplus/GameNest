using FluentValidation;
using GameNest.ReviewsService.Application.Commands.CommentCommands.CreateComment;

namespace GameNest.ReviewsService.Application.Validators.Comments
{
    public class CreateCommentCommandValidator : AbstractValidator<CreateCommentCommand>
    {
        public CreateCommentCommandValidator()
        {
            RuleFor(x => x.ReviewId)
                .NotEmpty();

            RuleFor(x => x.CustomerId)
                .NotEmpty();

            RuleFor(x => x.Text)
                .NotNull()
                .Must(t => !string.IsNullOrWhiteSpace(t.Value))
                .WithMessage("Comment text cannot be empty.")
                .Must(t => t.Value.Length <= 1000)
                .WithMessage("Comment text cannot exceed 1000 characters.");
        }
    }
}