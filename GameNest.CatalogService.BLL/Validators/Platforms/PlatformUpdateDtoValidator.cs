using FluentValidation;
using GameNest.CatalogService.BLL.DTOs.Platforms;

namespace GameNest.CatalogService.BLL.Validators.Platforms
{
    public class PlatformUpdateDtoValidator : AbstractValidator<PlatformUpdateDto>
    {
        public PlatformUpdateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name cannot be empty.")
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters.")
                .When(x => x.Name is not null);
        }
    }
}
