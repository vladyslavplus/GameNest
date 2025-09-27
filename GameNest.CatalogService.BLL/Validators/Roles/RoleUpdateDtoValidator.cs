using FluentValidation;
using GameNest.CatalogService.BLL.DTOs.Roles;

namespace GameNest.CatalogService.BLL.Validators.Roles
{
    public class RoleUpdateDtoValidator : AbstractValidator<RoleUpdateDto>
    {
        public RoleUpdateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name cannot be empty.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.")
                .When(x => x.Name is not null);
        }
    }
}
