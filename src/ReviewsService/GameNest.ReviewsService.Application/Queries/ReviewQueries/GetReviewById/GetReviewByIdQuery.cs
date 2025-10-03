using GameNest.ReviewsService.Application.Interfaces.Queries;
using GameNest.ReviewsService.Domain.Entities;

namespace GameNest.ReviewsService.Application.Queries.ReviewQueries.GetReviewById
{
    public class GetReviewByIdQuery : IQuery<Review?>
    {
        public string ReviewId { get; init; } = default!;
    }
}