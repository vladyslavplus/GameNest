using GameNest.ReviewsService.Domain.Common.Helpers;
using GameNest.ReviewsService.Domain.Entities;
using GameNest.ReviewsService.Domain.Entities.Parameters;
using GameNest.ReviewsService.Domain.Exceptions;
using GameNest.ReviewsService.Domain.Interfaces.Repositories;
using GameNest.ReviewsService.Domain.Interfaces.Services;
using GameNest.ReviewsService.Domain.ValueObjects;

namespace GameNest.ReviewsService.Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;

        public ReviewService(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task<PagedList<Review>> GetReviewsAsync(ReviewParameters parameters, CancellationToken cancellationToken = default)
        {
            return await _reviewRepository.GetReviewsAsync(parameters, cancellationToken);
        }

        public async Task<Review?> GetReviewByIdAsync(string reviewId, CancellationToken cancellationToken = default)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId, cancellationToken);
            if (review == null)
                throw new NotFoundException($"Review with Id '{reviewId}' not found.");
            return review;
        }

        public async Task AddReviewAsync(Review review, CancellationToken cancellationToken = default)
        {
            await _reviewRepository.AddAsync(review, cancellationToken);
        }

        public async Task UpdateReviewAsync(string reviewId, ReviewText? newText = null, Rating? newRating = null, string? requesterId = null, CancellationToken cancellationToken = default)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId, cancellationToken);
            if (review == null)
                throw new NotFoundException($"Review with Id '{reviewId}' not found.");

            if (!string.Equals(review.CustomerId, requesterId, StringComparison.OrdinalIgnoreCase))
                throw new ForbiddenException("User is not authorized to update this review.");

            if (newText != null)
                review.UpdateText(newText, requesterId ?? "system");

            if (newRating != null)
                review.UpdateRating(newRating, requesterId ?? "system");

            await _reviewRepository.UpdateAsync(review, cancellationToken);
        }

        public async Task DeleteReviewAsync(string reviewId, string requesterId, bool isAdmin = false, CancellationToken cancellationToken = default)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId, cancellationToken);
            if (review == null)
                throw new NotFoundException($"Review with Id '{reviewId}' not found.");

            if (isAdmin)
            {
                await _reviewRepository.DeleteAsync(reviewId, cancellationToken);
                return;
            }

            if (!string.Equals(review.CustomerId, requesterId, StringComparison.OrdinalIgnoreCase))
                throw new ForbiddenException("User is not authorized to delete this review.");

            await _reviewRepository.DeleteAsync(reviewId, cancellationToken);
        }
    }
}