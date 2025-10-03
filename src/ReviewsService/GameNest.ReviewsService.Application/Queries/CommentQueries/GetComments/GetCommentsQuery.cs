using GameNest.ReviewsService.Application.Interfaces.Queries;
using GameNest.ReviewsService.Domain.Common.Helpers;
using GameNest.ReviewsService.Domain.Entities;
using GameNest.ReviewsService.Domain.Entities.Parameters;

namespace GameNest.ReviewsService.Application.Queries.CommentQueries.GetComments
{
    public record GetCommentsQuery(CommentParameters Parameters) : IQuery<PagedList<Comment>>
    {
    }
}