using FluentValidation;
using GameNest.ReviewsService.Application.Commands.MediaCommands.CreateMedia;

namespace GameNest.ReviewsService.Application.Validators.Media
{
    public class CreateMediaCommandValidator : AbstractValidator<CreateMediaCommand>
    {
        public CreateMediaCommandValidator()
        {
            RuleFor(x => x.GameId)
                 .NotEmpty()
                 .WithMessage("GameId is required");

            RuleFor(x => x.CustomerId)
                .NotEmpty()
                .WithMessage("CustomerId is required");

            RuleFor(x => x.Url)
                .NotNull()
                .WithMessage("Url is required");
        }
    }
}