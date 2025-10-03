using FluentValidation;
using GameNest.ReviewsService.Application.Commands.MediaCommands.DeleteMedia;

namespace GameNest.ReviewsService.Application.Validators.Media
{
    public class DeleteMediaCommandValidator : AbstractValidator<DeleteMediaCommand>
    {
        public DeleteMediaCommandValidator()
        {
            RuleFor(x => x.MediaId)
                .NotEmpty()
                .WithMessage("MediaId is required.");
        }
    }
}