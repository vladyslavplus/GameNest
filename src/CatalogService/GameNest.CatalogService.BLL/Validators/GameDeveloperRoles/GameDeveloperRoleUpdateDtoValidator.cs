using FluentValidation;
using GameNest.CatalogService.BLL.DTOs.GameDeveloperRoles;

namespace GameNest.CatalogService.BLL.Validators.GameDeveloperRoles
{
    public class GameDeveloperRoleUpdateDtoValidator : AbstractValidator<GameDeveloperRoleUpdateDto>
    {
        public GameDeveloperRoleUpdateDtoValidator()
        {
            RuleFor(x => x.Seniority)
                .MaximumLength(50).WithMessage("Seniority must not exceed 50 characters.")
                .When(x => x.Seniority is not null);
        }
    }
}
