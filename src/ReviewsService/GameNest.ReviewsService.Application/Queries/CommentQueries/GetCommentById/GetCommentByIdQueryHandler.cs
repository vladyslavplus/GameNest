using GameNest.ReviewsService.Application.Interfaces.Queries;
using GameNest.ReviewsService.Domain.Entities;
using GameNest.ReviewsService.Domain.Interfaces.Services;

namespace GameNest.ReviewsService.Application.Queries.CommentQueries.GetCommentById
{
    public class GetCommentByIdQueryHandler : IQueryHandler<GetCommentByIdQuery, Comment?>
    {
        private readonly ICommentService _commentService;

        public GetCommentByIdQueryHandler(ICommentService commentService)
        {
            _commentService = commentService;
        }

        public async Task<Comment?> Handle(GetCommentByIdQuery request, CancellationToken cancellationToken)
        {
            return await _commentService.GetCommentByIdAsync(request.CommentId, cancellationToken);
        }
    }
}