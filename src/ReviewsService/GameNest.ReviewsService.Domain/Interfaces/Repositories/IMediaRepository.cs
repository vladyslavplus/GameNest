using GameNest.ReviewsService.Domain.Common;
using GameNest.ReviewsService.Domain.Common.Helpers;
using GameNest.ReviewsService.Domain.Entities;
using GameNest.ReviewsService.Domain.Entities.Parameters;

namespace GameNest.ReviewsService.Domain.Interfaces.Repositories
{
    public interface IMediaRepository : IMongoRepository<Media>
    {
        Task<PagedList<Media>> GetMediaAsync(MediaParameters parameters, CancellationToken cancellationToken = default);
    }
}