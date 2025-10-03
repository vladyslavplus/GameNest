using GameNest.ReviewsService.Application.Interfaces.Commands;
using GameNest.ReviewsService.Domain.Interfaces.Services;
using MediatR;

namespace GameNest.ReviewsService.Application.Commands.CommentCommands.AddReply
{
    public class AddReplyCommandHandler : ICommandHandler<AddReplyCommand>
    {
        private readonly ICommentService _commentService;

        public AddReplyCommandHandler(ICommentService commentService)
        {
            _commentService = commentService;
        }

        public async Task<Unit> Handle(AddReplyCommand request, CancellationToken cancellationToken)
        {
            await _commentService.AddReplyToCommentAsync(request.CommentId!, request.Reply, cancellationToken);
            return Unit.Value;
        }
    }
}