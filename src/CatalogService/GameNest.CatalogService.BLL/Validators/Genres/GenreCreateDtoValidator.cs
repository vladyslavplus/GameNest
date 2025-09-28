using FluentValidation;
using GameNest.CatalogService.BLL.DTOs.Genres;

namespace GameNest.CatalogService.BLL.Validators.Genres
{
    public class GenreCreateDtoValidator : AbstractValidator<GenreCreateDto>
    {
        public GenreCreateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");
        }
    }
}
