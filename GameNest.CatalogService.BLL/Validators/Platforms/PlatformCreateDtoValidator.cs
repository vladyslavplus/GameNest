using FluentValidation;
using GameNest.CatalogService.BLL.DTOs.Platforms;

namespace GameNest.CatalogService.BLL.Validators.Platforms
{
    public class PlatformCreateDtoValidator : AbstractValidator<PlatformCreateDto>
    {
        public PlatformCreateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");
        }
    }
}
