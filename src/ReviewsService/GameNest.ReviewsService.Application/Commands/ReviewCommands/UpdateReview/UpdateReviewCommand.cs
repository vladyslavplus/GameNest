using GameNest.ReviewsService.Application.Interfaces.Commands;
using GameNest.ReviewsService.Domain.ValueObjects;
using System.Text.Json.Serialization;

namespace GameNest.ReviewsService.Application.Commands.ReviewCommands.UpdateReview
{
    public record class UpdateReviewCommand : ICommand
    {
        [JsonIgnore]
        public string? ReviewId { get; init; }
        public ReviewText? NewText { get; init; }
        public Rating? NewRating { get; init; }
        [JsonIgnore]
        public Guid RequesterId { get; init; }
    }
}