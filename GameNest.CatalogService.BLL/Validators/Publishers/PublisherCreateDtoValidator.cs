using FluentValidation;
using GameNest.CatalogService.BLL.DTOs.Publishers;

namespace GameNest.CatalogService.BLL.Validators.Publishers
{
    public class PublisherCreateDtoValidator : AbstractValidator<PublisherCreateDto>
    {
        public PublisherCreateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("Type is required.")
                .MaximumLength(100).WithMessage("Type must not exceed 100 characters.");

            RuleFor(x => x.Country)
                .MaximumLength(100).WithMessage("Country must not exceed 100 characters.")
                .When(x => !string.IsNullOrEmpty(x.Country));

            RuleFor(x => x.Phone)
                .MaximumLength(20).WithMessage("Phone must not exceed 20 characters.")
                .When(x => !string.IsNullOrEmpty(x.Phone));
        }
    }
}
