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
            var review = new Review(request.GameId, request.CustomerId, request.Rating, request.Text, null!);
            await _reviewService.AddReviewAsync(review, cancellationToken);
            return review;
        }
    }
}