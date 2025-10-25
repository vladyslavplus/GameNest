using GameNest.ReviewsService.Application.Interfaces.Commands;
using GameNest.ReviewsService.Domain.Interfaces.Services;
using MediatR;

namespace GameNest.ReviewsService.Application.Commands.CommentCommands.UpdateReplyText
{
    public class UpdateReplyTextCommandHandler : ICommandHandler<UpdateReplyTextCommand>
    {
        private readonly ICommentService _commentService;

        public UpdateReplyTextCommandHandler(ICommentService commentService)
        {
            _commentService = commentService;
        }

        public async Task<Unit> Handle(UpdateReplyTextCommand request, CancellationToken cancellationToken)
        {
            await _commentService.UpdateReplyTextAsync(
                request.RequesterId,
                request.CommentId!,
                request.ReplyId!,
                request.NewText,
                request.IsAdmin,
                cancellationToken
            );

            return Unit.Value;
        }
    }
}
