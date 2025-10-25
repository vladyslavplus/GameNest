using GameNest.ReviewsService.Domain.Common.Helpers;
using GameNest.ReviewsService.Domain.Entities;
using GameNest.ReviewsService.Domain.Entities.Parameters;
using GameNest.ReviewsService.Domain.ValueObjects;

namespace GameNest.ReviewsService.Domain.Interfaces.Services
{
    public interface ICommentService
    {
        Task<PagedList<Comment>> GetCommentsAsync(CommentParameters parameters, CancellationToken cancellationToken = default);
        Task<Comment> GetCommentByIdAsync(string commentId, CancellationToken cancellationToken = default);
        Task AddCommentAsync(Comment comment, CancellationToken cancellationToken = default);
        Task UpdateCommentTextAsync(Guid requesterId, string commentId, ReviewText newText, CancellationToken cancellationToken = default);
        Task UpdateReplyTextAsync(Guid requesterId, string commentId, string replyId, ReviewText newText, bool isAdmin = false, CancellationToken cancellationToken = default);
        Task DeleteCommentAsync(Guid requesterId, string commentId, bool isAdmin = false, CancellationToken cancellationToken = default);
        Task AddReplyToCommentAsync(Guid requesterId, string commentId, ReviewText text, CancellationToken cancellationToken = default);
        Task DeleteReplyFromCommentAsync(Guid requesterId, string commentId, string replyId, bool isAdmin = false, CancellationToken cancellationToken = default);
    }
}