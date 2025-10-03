using GameNest.ReviewsService.Domain.Common;
using GameNest.ReviewsService.Domain.Common.Helpers;
using GameNest.ReviewsService.Domain.Entities;
using GameNest.ReviewsService.Domain.Entities.Parameters;

namespace GameNest.ReviewsService.Domain.Interfaces.Repositories
{
    public interface ICommentRepository : IMongoRepository<Comment>
    {
        Task<PagedList<Comment>> GetCommentsAsync(CommentParameters parameters, CancellationToken cancellationToken = default);
        Task AddReplyAsync(string commentId, Reply reply, CancellationToken cancellationToken = default);
        Task UpdateReplyAsync(string commentId, Reply reply, CancellationToken cancellationToken = default);
        Task DeleteReplyAsync(string commentId, string replyId, CancellationToken cancellationToken = default);
    }
}