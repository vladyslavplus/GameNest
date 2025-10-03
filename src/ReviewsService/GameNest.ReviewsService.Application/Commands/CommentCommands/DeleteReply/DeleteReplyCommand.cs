using GameNest.ReviewsService.Application.Interfaces.Commands;

namespace GameNest.ReviewsService.Application.Commands.CommentCommands.DeleteReply
{
    public class DeleteReplyCommand : ICommand
    {
        public string CommentId { get; init; } = default!;
        public string ReplyId { get; init; } = default!;
    }
}