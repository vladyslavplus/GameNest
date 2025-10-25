using GameNest.ReviewsService.Application.Interfaces.Commands;
using GameNest.ReviewsService.Domain.ValueObjects;
using System.Text.Json.Serialization;

namespace GameNest.ReviewsService.Application.Commands.CommentCommands.UpdateReplyText
{
    public record class UpdateReplyTextCommand : ICommand
    {
        [JsonIgnore]
        public string? CommentId { get; init; }
        [JsonIgnore]
        public string? ReplyId { get; init; }
        public ReviewText NewText { get; init; } = default!;
        [JsonIgnore]
        public Guid RequesterId { get; init; }
        [JsonIgnore]
        public bool IsAdmin { get; init; }
    }
}