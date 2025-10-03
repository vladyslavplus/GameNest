using GameNest.ReviewsService.Application.Interfaces.Commands;
using GameNest.ReviewsService.Domain.ValueObjects;
using System.Text.Json.Serialization;

namespace GameNest.ReviewsService.Application.Commands.CommentCommands.UpdateCommentText
{
    public record class UpdateCommentTextCommand : ICommand
    {
        [JsonIgnore]
        public string? CommentId { get; init; }
        public ReviewText NewText { get; init; } = default!;
    }
}