using GameNest.ReviewsService.Domain.Common.Helpers;
using GameNest.ReviewsService.Domain.Entities;
using GameNest.ReviewsService.Domain.Entities.Parameters;
using GameNest.ReviewsService.Domain.ValueObjects;

namespace GameNest.ReviewsService.Domain.Interfaces.Services
{
    public interface IReviewService
    {
        Task<PagedList<Review>> GetReviewsAsync(ReviewParameters parameters, CancellationToken cancellationToken = default);
        Task<Review?> GetReviewByIdAsync(string reviewId, CancellationToken cancellationToken = default);
        Task AddReviewAsync(Review review, CancellationToken cancellationToken = default);
        Task UpdateReviewAsync(string reviewId, ReviewText? newText = null, Rating? newRating = null, CancellationToken cancellationToken = default);
        Task AddReplyToReviewAsync(string reviewId, Reply reply, CancellationToken cancellationToken = default);
        Task DeleteReviewAsync(string reviewId, CancellationToken cancellationToken = default);
        Task DeleteReplyFromReviewAsync(string reviewId, string replyId, CancellationToken cancellationToken = default);
    }
}
