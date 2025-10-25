using GameNest.ReviewsService.Application.Interfaces.Commands;
using System.Text.Json.Serialization;

namespace GameNest.ReviewsService.Application.Commands.CommentCommands.DeleteComment
{
    public class DeleteCommentCommand : ICommand
    {
        [JsonIgnore]
        public Guid RequesterId { get; init; }
        public string CommentId { get; init; } = default!;
        [JsonIgnore]
        public bool IsAdmin { get; init; }
    }
}