using FluentValidation;
using GameNest.CatalogService.BLL.DTOs.Roles;

namespace GameNest.CatalogService.BLL.Validators.Roles
{
    public class RoleCreateDtoValidator : AbstractValidator<RoleCreateDto>
    {
        public RoleCreateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");
        }
    }
}
