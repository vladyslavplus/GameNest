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

        public async Task UpdateReviewAsync(string reviewId, ReviewText? newText = null, Rating? newRating = null, CancellationToken cancellationToken = default)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId, cancellationToken);
            if (review == null)
                throw new NotFoundException($"Review with Id '{reviewId}' not found.");

            if (newText != null)
                review.UpdateText(newText);

            if (newRating != null)
                review.UpdateRating(newRating);

            await _reviewRepository.UpdateAsync(review, cancellationToken);
        }

        public async Task AddReplyToReviewAsync(string reviewId, Reply reply, CancellationToken cancellationToken = default)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId, cancellationToken);
            if (review == null)
                throw new NotFoundException($"Review with Id '{reviewId}' not found.");

            review.AddReply(reply);
            await _reviewRepository.UpdateAsync(review, cancellationToken);
        }

        public async Task DeleteReviewAsync(string reviewId, CancellationToken cancellationToken = default)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId, cancellationToken);
            if (review == null)
                throw new NotFoundException($"Review with Id '{reviewId}' not found.");

            await _reviewRepository.DeleteAsync(reviewId, cancellationToken);
        }

        public async Task DeleteReplyFromReviewAsync(string reviewId, string replyId, CancellationToken cancellationToken = default)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId, cancellationToken);
            if (review == null)
                throw new NotFoundException($"Review with Id '{reviewId}' not found.");

            var reply = review.Replies.FirstOrDefault(r => r.Id == replyId);
            if (reply == null)
                throw new NotFoundException($"Reply with Id '{replyId}' not found in review '{reviewId}'.");

            review.Replies.Remove(reply);
            await _reviewRepository.UpdateAsync(review, cancellationToken);
        }
    }
}