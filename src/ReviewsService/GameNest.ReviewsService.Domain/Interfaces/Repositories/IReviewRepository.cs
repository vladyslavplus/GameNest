using GameNest.ReviewsService.Domain.Common;
using GameNest.ReviewsService.Domain.Common.Helpers;
using GameNest.ReviewsService.Domain.Entities;
using GameNest.ReviewsService.Domain.Entities.Parameters;

namespace GameNest.ReviewsService.Domain.Interfaces.Repositories
{
    public interface IReviewRepository : IMongoRepository<Review>
    {
        Task<PagedList<Review>> GetReviewsAsync(ReviewParameters parameters, CancellationToken cancellationToken = default);
        Task AddReplyAsync(string reviewId, Reply reply, CancellationToken cancellationToken = default);
        Task UpdateReplyAsync(string reviewId, Reply reply, CancellationToken cancellationToken = default);
        Task DeleteReplyAsync(string reviewId, string replyId, CancellationToken cancellationToken = default);
    }
}
