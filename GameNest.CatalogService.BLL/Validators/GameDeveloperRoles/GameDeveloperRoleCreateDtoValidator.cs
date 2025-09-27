using FluentValidation;
using GameNest.CatalogService.BLL.DTOs.GameDeveloperRoles;

namespace GameNest.CatalogService.BLL.Validators.GameDeveloperRoles
{
    public class GameDeveloperRoleCreateDtoValidator : AbstractValidator<GameDeveloperRoleCreateDto>
    {
        public GameDeveloperRoleCreateDtoValidator()
        {
            RuleFor(x => x.GameId)
                .NotEmpty().WithMessage("GameId is required.");

            RuleFor(x => x.DeveloperId)
                .NotEmpty().WithMessage("DeveloperId is required.");

            RuleFor(x => x.RoleId)
                .NotEmpty().WithMessage("RoleId is required.");

            RuleFor(x => x.Seniority)
                .NotEmpty().WithMessage("Seniority is required.")
                .MaximumLength(50).WithMessage("Seniority must not exceed 50 characters.");
        }
    }
}