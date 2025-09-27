using FluentValidation;
using GameNest.CatalogService.BLL.DTOs.Genres;

namespace GameNest.CatalogService.BLL.Validators.Genres
{
    public class GenreUpdateDtoValidator : AbstractValidator<GenreUpdateDto>
    {
        public GenreUpdateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name cannot be empty.")
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters.")
                .When(x => x.Name is not null);
        }
    }
}
