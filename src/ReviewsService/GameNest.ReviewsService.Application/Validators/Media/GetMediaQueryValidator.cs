using FluentValidation;
using GameNest.ReviewsService.Application.Queries.MediaQueries.GetMedia;

namespace GameNest.ReviewsService.Application.Validators.Media
{
    public class GetMediaQueryValidator : AbstractValidator<GetMediaQuery>
    {
        public GetMediaQueryValidator()
        {
            RuleFor(x => x.Parameters.PageNumber).GreaterThan(0);
            RuleFor(x => x.Parameters.PageSize).GreaterThan(0);
        }
    }
}