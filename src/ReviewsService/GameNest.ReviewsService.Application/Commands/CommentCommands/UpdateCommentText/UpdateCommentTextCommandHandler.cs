using GameNest.ReviewsService.Application.Interfaces.Commands;
using GameNest.ReviewsService.Domain.Interfaces.Services;
using MediatR;

namespace GameNest.ReviewsService.Application.Commands.CommentCommands.UpdateCommentText
{
    public class UpdateCommentTextCommandHandler : ICommandHandler<UpdateCommentTextCommand>
    {
        private readonly ICommentService _commentService;

        public UpdateCommentTextCommandHandler(ICommentService commentService)
        {
            _commentService = commentService;
        }

        public async Task<Unit> Handle(UpdateCommentTextCommand request, CancellationToken cancellationToken)
        {
            await _commentService.UpdateCommentTextAsync(request.CommentId!, request.NewText, cancellationToken);
            return Unit.Value;
        }
    }
}