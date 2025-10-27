using GameNest.ReviewsService.Application.Interfaces.Commands;
using GameNest.ReviewsService.Domain.Entities;
using GameNest.ReviewsService.Domain.ValueObjects;
using System.Text.Json.Serialization;

namespace GameNest.ReviewsService.Application.Commands.ReviewCommands.AddReview
{
    public record class AddReviewCommand : ICommand<Review>
    {
        public string GameId { get; init; } = default!;
        [JsonIgnore]
        public Guid CustomerId { get; init; }
        public Rating Rating { get; init; } = default!;
        public ReviewText Text { get; init; } = default!;
    }
}