using GameNest.ReviewsService.Domain.Common.Helpers;
using GameNest.ReviewsService.Domain.Entities;
using GameNest.ReviewsService.Domain.Entities.Parameters;
using GameNest.ReviewsService.Domain.ValueObjects;

namespace GameNest.ReviewsService.Domain.Interfaces.Services
{
    public interface ICommentService
    {
        Task<PagedList<Comment>> GetCommentsAsync(CommentParameters parameters, CancellationToken cancellationToken = default);
        Task<Comment?> GetCommentByIdAsync(string commentId, CancellationToken cancellationToken = default);
        Task AddCommentAsync(Comment comment, CancellationToken cancellationToken = default);
        Task UpdateCommentTextAsync(string commentId, ReviewText newText, CancellationToken cancellationToken = default);
        Task DeleteCommentAsync(string commentId, CancellationToken cancellationToken = default);
        Task AddReplyToCommentAsync(string commentId, Reply reply, CancellationToken cancellationToken = default);
        Task DeleteReplyFromCommentAsync(string commentId, string replyId, CancellationToken cancellationToken = default);
    }
}