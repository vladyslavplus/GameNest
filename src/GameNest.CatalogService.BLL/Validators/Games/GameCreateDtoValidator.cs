using FluentValidation;
using GameNest.CatalogService.BLL.DTOs.Games;

namespace GameNest.CatalogService.BLL.Validators.Games
{
    public class GameCreateDtoValidator : AbstractValidator<GameCreateDto>
    {
        public GameCreateDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

            RuleFor(x => x.ReleaseDate)
                .NotEmpty().When(x => x.ReleaseDate.HasValue) 
                .WithMessage("Release date is invalid.");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price must be greater than or equal to 0.");

            RuleFor(x => x.PublisherId)
                .Must(id => !id.HasValue || id != Guid.Empty)
                .WithMessage("PublisherId must be a valid GUID if provided.");
        }
    }
}
