using GameNest.ReviewsService.Domain.Common.Helpers;
using GameNest.ReviewsService.Domain.Entities;
using GameNest.ReviewsService.Domain.Entities.Parameters;
using GameNest.ReviewsService.Domain.Exceptions;
using GameNest.ReviewsService.Domain.Interfaces.Repositories;
using GameNest.ReviewsService.Domain.Interfaces.Services;
using GameNest.ReviewsService.Domain.ValueObjects;

namespace GameNest.ReviewsService.Application.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;

        public CommentService(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        public async Task<PagedList<Comment>> GetCommentsAsync(CommentParameters parameters, CancellationToken cancellationToken = default)
        {
            return await _commentRepository.GetCommentsAsync(parameters, cancellationToken);
        }

        public async Task<Comment> GetCommentByIdAsync(string commentId, CancellationToken cancellationToken = default)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId, cancellationToken);
            if (comment == null)
                throw new NotFoundException($"Comment with Id '{commentId}' not found.");

            return comment;
        }

        public async Task AddCommentAsync(Comment comment, CancellationToken cancellationToken = default)
        {
            await _commentRepository.AddAsync(comment, cancellationToken);
        }

        public async Task UpdateCommentTextAsync(Guid requesterId, string commentId, ReviewText newText, CancellationToken cancellationToken = default)
        {
            var comment = await GetCommentByIdAsync(commentId, cancellationToken);

            if (comment.CustomerId != requesterId.ToString())
                throw new ForbiddenException("User is not authorized to update this comment.");

            comment.UpdateText(newText, requesterId.ToString());
            await _commentRepository.UpdateAsync(comment, cancellationToken);
        }

        public async Task UpdateReplyTextAsync(Guid requesterId, string commentId, string replyId, ReviewText newText, bool isAdmin = false, CancellationToken cancellationToken = default)
        {
            var comment = await GetCommentByIdAsync(commentId, cancellationToken);

            var reply = comment.Replies.FirstOrDefault(r => r.Id == replyId);
            if (reply == null)
                throw new NotFoundException($"Reply with Id '{replyId}' not found in comment '{commentId}'.");

            if (isAdmin)
            {
                reply.UpdateText(newText);
                await _commentRepository.UpdateReplyAsync(commentId, reply, cancellationToken);
                return;
            }

            if (reply.CustomerId != requesterId.ToString())
                throw new ForbiddenException("User is not authorized to update this reply.");

            reply.UpdateText(newText);
            await _commentRepository.UpdateReplyAsync(commentId, reply, cancellationToken);
        }

        public async Task DeleteCommentAsync(Guid requesterId, string commentId, bool isAdmin = false, CancellationToken cancellationToken = default)
        {
            var comment = await GetCommentByIdAsync(commentId, cancellationToken);

            if (isAdmin)
            {
                await _commentRepository.DeleteAsync(commentId, cancellationToken);
                return;
            }

            if (comment.CustomerId != requesterId.ToString())
                throw new ForbiddenException("User is not authorized to delete this comment.");

            await _commentRepository.DeleteAsync(commentId, cancellationToken);
        }

        public async Task AddReplyToCommentAsync(Guid requesterId, string commentId, ReviewText text, CancellationToken cancellationToken = default)
        {
            await GetCommentByIdAsync(commentId, cancellationToken);

            var newReply = new Reply(requesterId.ToString(), text);
            await _commentRepository.AddReplyAsync(commentId, newReply, cancellationToken);
        }

        public async Task DeleteReplyFromCommentAsync(Guid requesterId, string commentId, string replyId, bool isAdmin = false, CancellationToken cancellationToken = default)
        {
            var comment = await GetCommentByIdAsync(commentId, cancellationToken);

            var reply = comment.Replies.FirstOrDefault(r => r.Id == replyId);
            if (reply == null)
                throw new NotFoundException($"Reply with Id '{replyId}' not found in comment '{commentId}'.");

            if (isAdmin)
            {
                await _commentRepository.DeleteReplyAsync(commentId, replyId, cancellationToken);
                return;
            }

            var requesterIdString = requesterId.ToString();
            bool isReplyOwner = reply.CustomerId == requesterIdString;
            bool isCommentOwner = comment.CustomerId == requesterIdString;

            if (!isReplyOwner && !isCommentOwner)
                throw new ForbiddenException("User is not authorized to delete this reply.");

            await _commentRepository.DeleteReplyAsync(commentId, replyId, cancellationToken);
        }
    }
}