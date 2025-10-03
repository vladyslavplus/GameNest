using GameNest.ReviewsService.Application.Interfaces.Queries;
using GameNest.ReviewsService.Domain.Entities;
using GameNest.ReviewsService.Domain.Interfaces.Services;

namespace GameNest.ReviewsService.Application.Queries.ReviewQueries.GetReviewById
{
    public class GetReviewByIdQueryHandler : IQueryHandler<GetReviewByIdQuery, Review?>
    {
        private readonly IReviewService _reviewService;

        public GetReviewByIdQueryHandler(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        public async Task<Review?> Handle(GetReviewByIdQuery request, CancellationToken cancellationToken)
        {
            return await _reviewService.GetReviewByIdAsync(request.ReviewId, cancellationToken);
        }
    }
}