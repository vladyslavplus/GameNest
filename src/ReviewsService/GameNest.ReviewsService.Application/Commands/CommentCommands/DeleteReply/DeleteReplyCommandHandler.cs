using GameNest.ReviewsService.Application.Interfaces.Commands;
using GameNest.ReviewsService.Domain.Interfaces.Services;
using MediatR;

namespace GameNest.ReviewsService.Application.Commands.CommentCommands.DeleteReply
{
    public class DeleteReplyCommandHandler : ICommandHandler<DeleteReplyCommand>
    {
        private readonly ICommentService _commentService;

        public DeleteReplyCommandHandler(ICommentService commentService)
        {
            _commentService = commentService;
        }

        public async Task<Unit> Handle(DeleteReplyCommand request, CancellationToken cancellationToken)
        {
            await _commentService.DeleteReplyFromCommentAsync(request.RequesterId, request.CommentId, request.ReplyId, request.IsAdmin, cancellationToken);
            return Unit.Value;
        }
    }
}