using GameNest.ReviewsService.Application.Interfaces.Queries;
using GameNest.ReviewsService.Domain.Common.Helpers;
using GameNest.ReviewsService.Domain.Entities;
using GameNest.ReviewsService.Domain.Interfaces.Services;

namespace GameNest.ReviewsService.Application.Queries.CommentQueries.GetComments
{

    public class GetCommentsQueryHandler : IQueryHandler<GetCommentsQuery, PagedList<Comment>>
    {
        private readonly ICommentService _commentService;

        public GetCommentsQueryHandler(ICommentService commentService)
        {
            _commentService = commentService;
        }

        public async Task<PagedList<Comment>> Handle(GetCommentsQuery request, CancellationToken cancellationToken)
        {
            return await _commentService.GetCommentsAsync(request.Parameters, cancellationToken);
        }
    }
}