using GameNest.ReviewsService.Application.Interfaces.Commands;
using System.Text.Json.Serialization;

namespace GameNest.ReviewsService.Application.Commands.ReviewCommands.DeleteReview
{
    public record class DeleteReviewCommand : ICommand
    {
        [JsonIgnore]
        public string ReviewId { get; init; } = default!;

        [JsonIgnore]
        public Guid RequesterId { get; init; }

        [JsonIgnore]
        public bool IsAdmin { get; init; }
    }
}