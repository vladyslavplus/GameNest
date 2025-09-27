using FluentValidation;
using GameNest.CatalogService.BLL.DTOs.GameGenres;

namespace GameNest.CatalogService.BLL.Validators.GameGenres
{
    public class GameGenreUpdateDtoValidator : AbstractValidator<GameGenreUpdateDto>
    {
        public GameGenreUpdateDtoValidator()
        {
            RuleFor(x => x.GameId)
                .NotEmpty().WithMessage("GameId cannot be empty.")
                .When(x => x.GameId.HasValue);

            RuleFor(x => x.GenreId)
                .NotEmpty().WithMessage("GenreId cannot be empty.")
                .When(x => x.GenreId.HasValue);
        }
    }
}
