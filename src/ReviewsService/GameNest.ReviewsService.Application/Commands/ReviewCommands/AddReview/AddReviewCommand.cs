using GameNest.ReviewsService.Application.Interfaces.Commands;
using GameNest.ReviewsService.Domain.Entities;
using GameNest.ReviewsService.Domain.ValueObjects;

namespace GameNest.ReviewsService.Application.Commands.ReviewCommands.AddReview
{
    public class AddReviewCommand : ICommand<Review>
    {
        public string GameId { get; init; } = default!;
        public string CustomerId { get; init; } = default!;
        public Rating Rating { get; init; } = default!;
        public ReviewText Text { get; init; } = default!;
    }
}