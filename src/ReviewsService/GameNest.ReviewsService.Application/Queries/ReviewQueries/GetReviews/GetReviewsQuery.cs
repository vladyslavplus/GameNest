using GameNest.ReviewsService.Application.Interfaces.Queries;
using GameNest.ReviewsService.Domain.Common.Helpers;
using GameNest.ReviewsService.Domain.Entities;
using GameNest.ReviewsService.Domain.Entities.Parameters;

namespace GameNest.ReviewsService.Application.Queries.ReviewQueries.GetReviews
{
    public class GetReviewsQuery : IQuery<PagedList<Review>>
    {
        public ReviewParameters Parameters { get; init; } = default!;
    }
}