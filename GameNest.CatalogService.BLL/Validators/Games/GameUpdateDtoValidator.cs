using FluentValidation;
using GameNest.CatalogService.BLL.DTOs.Games;

namespace GameNest.CatalogService.BLL.Validators.Games
{
    public class GameUpdateDtoValidator : AbstractValidator<GameUpdateDto>
    {
        public GameUpdateDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title cannot be empty.")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters.")
                .When(x => x.Title is not null);

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.ReleaseDate)
                .NotEmpty().When(x => x.ReleaseDate.HasValue)
                .WithMessage("Release date is invalid.");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0)
                .When(x => x.Price.HasValue)
                .WithMessage("Price must be greater than or equal to 0.");

            RuleFor(x => x.PublisherId)
                .NotEmpty()
                .When(x => x.PublisherId.HasValue)
                .WithMessage("PublisherId cannot be empty.");
        }
    }
}
