using GameNest.ReviewsService.Application.Interfaces.Commands;
using GameNest.ReviewsService.Domain.Entities;
using GameNest.ReviewsService.Domain.Interfaces.Services;

namespace GameNest.ReviewsService.Application.Commands.ReviewCommands.AddReview
{
    public class AddReviewCommandHandler : ICommandHandler<AddReviewCommand, Review>
    {
        private readonly IReviewService _reviewService;

        public AddReviewCommandHandler(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        public async Task<Review> Handle(AddReviewCommand request, CancellationToken cancellationToken)
        {
            var review = new Review(
                gameId: request.GameId,
                customerId: request.CustomerId.ToString(),
                rating: request.Rating,
                text: request.Text,
                createdBy: request.CustomerId.ToString()
            );

            await _reviewService.AddReviewAsync(review, cancellationToken);
            return review;
        }
    }
}