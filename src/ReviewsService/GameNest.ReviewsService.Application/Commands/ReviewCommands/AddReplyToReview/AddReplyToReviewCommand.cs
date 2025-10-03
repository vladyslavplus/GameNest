using GameNest.ReviewsService.Application.Interfaces.Commands;
using GameNest.ReviewsService.Domain.Entities;
using System.Text.Json.Serialization;

namespace GameNest.ReviewsService.Application.Commands.ReviewCommands.AddReplyToReview
{
    public record class AddReplyToReviewCommand : ICommand
    {
        [JsonIgnore]
        public string? ReviewId { get; init; }
        public Reply Reply { get; init; } = default!;
    }
}