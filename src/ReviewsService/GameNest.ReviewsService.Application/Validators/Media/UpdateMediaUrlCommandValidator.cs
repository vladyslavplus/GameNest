using FluentValidation;
using GameNest.ReviewsService.Application.Commands.MediaCommands.UpdateMediaUrl;

namespace GameNest.ReviewsService.Application.Validators.Media
{
    public class UpdateMediaUrlCommandValidator : AbstractValidator<UpdateMediaUrlCommand>
    {
        public UpdateMediaUrlCommandValidator()
        {
            RuleFor(x => x.NewUrl).NotEmpty().WithMessage("NewUrl is required.");
        }
    }
}