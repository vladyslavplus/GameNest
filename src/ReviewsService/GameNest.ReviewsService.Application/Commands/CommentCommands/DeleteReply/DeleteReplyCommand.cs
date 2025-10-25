using GameNest.ReviewsService.Application.Interfaces.Commands;
using System.Text.Json.Serialization;

namespace GameNest.ReviewsService.Application.Commands.CommentCommands.DeleteReply
{
    public class DeleteReplyCommand : ICommand
    {
        [JsonIgnore]
        public Guid RequesterId { get; init; }
        public string CommentId { get; init; } = default!;
        public string ReplyId { get; init; } = default!;
        [JsonIgnore]
        public bool IsAdmin { get; init; }
    }
}