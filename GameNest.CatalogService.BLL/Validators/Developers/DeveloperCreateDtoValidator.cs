using FluentValidation;
using GameNest.CatalogService.BLL.DTOs.Developers;

namespace GameNest.CatalogService.BLL.Validators.Developers
{
    public class DeveloperCreateDtoValidator : AbstractValidator<DeveloperCreateDto>
    {
        public DeveloperCreateDtoValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("FullName is required.")
                .MaximumLength(200).WithMessage("FullName must not exceed 200 characters.");

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Invalid email format.")
                .When(x => !string.IsNullOrEmpty(x.Email));

            RuleFor(x => x.Country)
                .MaximumLength(100).WithMessage("Country must not exceed 100 characters.");
        }
    }
}
