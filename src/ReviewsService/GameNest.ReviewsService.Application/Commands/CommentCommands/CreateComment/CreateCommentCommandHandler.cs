using GameNest.ReviewsService.Application.Interfaces.Commands;
using GameNest.ReviewsService.Domain.Entities;
using GameNest.ReviewsService.Domain.Interfaces.Services;

namespace GameNest.ReviewsService.Application.Commands.CommentCommands.CreateComment
{
    public class CreateCommentCommandHandler : ICommandHandler<CreateCommentCommand, Comment>
    {
        private readonly ICommentService _commentService;

        public CreateCommentCommandHandler(ICommentService commentService)
        {
            _commentService = commentService;
        }

        public async Task<Comment> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
        {
            var comment = new Comment(request.ReviewId, request.CustomerId.ToString(), request.Text, request.CustomerId.ToString());
            await _commentService.AddCommentAsync(comment, cancellationToken);
            return comment;
        }
    }
}