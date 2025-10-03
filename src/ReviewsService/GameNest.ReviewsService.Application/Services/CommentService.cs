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

        public async Task<Comment?> GetCommentByIdAsync(string commentId, CancellationToken cancellationToken = default)
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

        public async Task UpdateCommentTextAsync(string commentId, ReviewText newText, CancellationToken cancellationToken = default)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId, cancellationToken);
            if (comment == null)
                throw new NotFoundException($"Comment with Id '{commentId}' not found.");

            comment.UpdateText(newText); 
            await _commentRepository.UpdateAsync(comment, cancellationToken);
        }

        public async Task DeleteCommentAsync(string commentId, CancellationToken cancellationToken = default)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId, cancellationToken);
            if (comment == null)
                throw new NotFoundException($"Comment with Id '{commentId}' not found.");

            await _commentRepository.DeleteAsync(commentId, cancellationToken);
        }

        public async Task AddReplyToCommentAsync(string commentId, Reply reply, CancellationToken cancellationToken = default)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId, cancellationToken);
            if (comment == null)
                throw new NotFoundException($"Comment with Id '{commentId}' not found.");

            await _commentRepository.AddReplyAsync(commentId, reply, cancellationToken);
        }

        public async Task DeleteReplyFromCommentAsync(string commentId, string replyId, CancellationToken cancellationToken = default)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId, cancellationToken);
            if (comment == null)
                throw new NotFoundException($"Comment with Id '{commentId}' not found.");

            var replyExists = comment.Replies.Any(r => r.Id == replyId);
            if (!replyExists)
                throw new NotFoundException($"Reply with Id '{replyId}' not found in comment '{commentId}'.");

            await _commentRepository.DeleteReplyAsync(commentId, replyId, cancellationToken);
        }
    }
}