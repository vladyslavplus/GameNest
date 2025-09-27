using FluentValidation;
using GameNest.CatalogService.BLL.DTOs.GameGenres;

namespace GameNest.CatalogService.BLL.Validators.GameGenres
{
    public class GameGenreCreateDtoValidator : AbstractValidator<GameGenreCreateDto>
    {
        public GameGenreCreateDtoValidator()
        {
            RuleFor(x => x.GameId)
                .NotEmpty().WithMessage("GameId is required.");

            RuleFor(x => x.GenreId)
                .NotEmpty().WithMessage("GenreId is required.");
        }
    }
}
