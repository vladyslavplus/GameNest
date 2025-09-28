using FluentValidation;
using GameNest.CatalogService.BLL.DTOs.GamePlatforms;

namespace GameNest.CatalogService.BLL.Validators.GamePlatforms
{
    public class GamePlatformCreateDtoValidator : AbstractValidator<GamePlatformCreateDto>
    {
        public GamePlatformCreateDtoValidator()
        {
            RuleFor(x => x.GameId)
                .NotEmpty().WithMessage("GameId is required.");

            RuleFor(x => x.PlatformId)
                .NotEmpty().WithMessage("PlatformId is required.");
        }
    }
}
