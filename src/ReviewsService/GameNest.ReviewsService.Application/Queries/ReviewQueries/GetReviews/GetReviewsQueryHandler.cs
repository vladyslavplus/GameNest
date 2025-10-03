using GameNest.ReviewsService.Application.Interfaces.Queries;
using GameNest.ReviewsService.Domain.Common.Helpers;
using GameNest.ReviewsService.Domain.Entities;
using GameNest.ReviewsService.Domain.Interfaces.Services;

namespace GameNest.ReviewsService.Application.Queries.ReviewQueries.GetReviews
{
    public class GetReviewsQueryHandler : IQueryHandler<GetReviewsQuery, PagedList<Review>>
    {
        private readonly IReviewService _reviewService;

        public GetReviewsQueryHandler(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        public async Task<PagedList<Review>> Handle(GetReviewsQuery request, CancellationToken cancellationToken)
        {
            return await _reviewService.GetReviewsAsync(request.Parameters, cancellationToken);
        }
    }
}