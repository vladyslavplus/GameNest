using FluentValidation;
using GameNest.CatalogService.BLL.DTOs.Publishers;

namespace GameNest.CatalogService.BLL.Validators.Publishers
{
    public class PublisherUpdateDtoValidator : AbstractValidator<PublisherUpdateDto>
    {
        public PublisherUpdateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name cannot be empty.")
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters.")
                .When(x => x.Name is not null);

            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("Type cannot be empty.")
                .MaximumLength(100).WithMessage("Type must not exceed 100 characters.")
                .When(x => x.Type is not null);

            RuleFor(x => x.Country)
                .MaximumLength(100).WithMessage("Country must not exceed 100 characters.")
                .When(x => !string.IsNullOrEmpty(x.Country));

            RuleFor(x => x.Phone)
                .MaximumLength(20).WithMessage("Phone must not exceed 20 characters.")
                .When(x => !string.IsNullOrEmpty(x.Phone));
        }
    }
}
