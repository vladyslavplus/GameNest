using FluentValidation;
using GameNest.CatalogService.BLL.DTOs.GamePlatforms;

namespace GameNest.CatalogService.BLL.Validators.GamePlatforms
{
    public class GamePlatformUpdateDtoValidator : AbstractValidator<GamePlatformUpdateDto>
    {
        public GamePlatformUpdateDtoValidator()
        {
            RuleFor(x => x.GameId)
                .NotEmpty().WithMessage("GameId cannot be empty.")
                .When(x => x.GameId.HasValue);

            RuleFor(x => x.PlatformId)
                .NotEmpty().WithMessage("PlatformId cannot be empty.")
                .When(x => x.PlatformId.HasValue);
        }
    }
}
