using GameNest.ReviewsService.Application.Interfaces.Queries;
using GameNest.ReviewsService.Domain.Entities;

namespace GameNest.ReviewsService.Application.Queries.CommentQueries.GetCommentById
{
    public class GetCommentByIdQuery : IQuery<Comment?>
    {
        public string CommentId { get; init; } = default!;
    }
}