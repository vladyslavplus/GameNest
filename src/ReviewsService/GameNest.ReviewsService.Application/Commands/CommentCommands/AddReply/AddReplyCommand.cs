using GameNest.ReviewsService.Application.Interfaces.Commands;
using GameNest.ReviewsService.Domain.ValueObjects;
using System.Text.Json.Serialization;

namespace GameNest.ReviewsService.Application.Commands.CommentCommands.AddReply
{
    public record class AddReplyCommand : ICommand
    {
        [JsonIgnore]
        public string? CommentId { get; init; }
        [JsonIgnore]
        public Guid RequesterId { get; init; }
        public ReviewText Text { get; init; } = default!;
    }
}