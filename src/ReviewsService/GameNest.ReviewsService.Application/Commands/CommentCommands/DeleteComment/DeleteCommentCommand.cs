using GameNest.ReviewsService.Application.Interfaces.Commands;

namespace GameNest.ReviewsService.Application.Commands.CommentCommands.DeleteComment
{
    public class DeleteCommentCommand : ICommand
    {
        public string CommentId { get; init; } = default!;
    }
}