using GameNest.GrpcClients.Interfaces;
using GameNest.ReviewsService.Domain.Common.Helpers;
using GameNest.ReviewsService.Domain.Entities;
using GameNest.ReviewsService.Domain.Entities.Parameters;
using GameNest.ReviewsService.Domain.Exceptions;
using GameNest.ReviewsService.Domain.Interfaces.Repositories;
using GameNest.ReviewsService.Domain.Interfaces.Services;
using GameNest.ReviewsService.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace GameNest.ReviewsService.Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IGameGrpcClient _gameClient;
        private readonly ILogger<ReviewService> _logger;

        public ReviewService(
            IReviewRepository reviewRepository,
            IGameGrpcClient gameClient,
            ILogger<ReviewService> logger)
        {
            _reviewRepository = reviewRepository;
            _gameClient = gameClient;
            _logger = logger;
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
            var game = await _gameClient.GetGameByIdAsync(Guid.Parse(review.GameId), cancellationToken);
            if (game == null)
            {
                _logger.LogWarning("Attempted to create review for non-existent game {GameId}.", review.GameId);
                throw new NotFoundException($"Game with Id '{review.GameId}' not found in CatalogService.");
            }

            await _reviewRepository.AddAsync(review, cancellationToken);
        }

        public async Task UpdateReviewAsync(string reviewId, ReviewText? newText = null, Rating? newRating = null, Guid? requesterId = null, CancellationToken cancellationToken = default)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId, cancellationToken);
            if (review == null)
                throw new NotFoundException($"Review with Id '{reviewId}' not found.");

            var requester = requesterId?.ToString();

            if (!string.Equals(review.CustomerId, requester, StringComparison.OrdinalIgnoreCase))
                throw new ForbiddenException("User is not authorized to update this review.");

            if (newText != null)
                review.UpdateText(newText, requester ?? "system");

            if (newRating != null)
                review.UpdateRating(newRating, requester ?? "system");

            await _reviewRepository.UpdateAsync(review, cancellationToken);
        }

        public async Task DeleteReviewAsync(string reviewId, Guid requesterId, bool isAdmin = false, CancellationToken cancellationToken = default)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId, cancellationToken);
            if (review == null)
                throw new NotFoundException($"Review with Id '{reviewId}' not found.");

            if (isAdmin)
            {
                await _reviewRepository.DeleteAsync(reviewId, cancellationToken);
                return;
            }

            if (!string.Equals(review.CustomerId, requesterId.ToString(), StringComparison.OrdinalIgnoreCase))
                throw new ForbiddenException("User is not authorized to delete this review.");

            await _reviewRepository.DeleteAsync(reviewId, cancellationToken);
        }
    }
}