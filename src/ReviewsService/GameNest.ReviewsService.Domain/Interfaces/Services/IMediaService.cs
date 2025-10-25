using GameNest.ReviewsService.Domain.Common.Helpers;
using GameNest.ReviewsService.Domain.Entities;
using GameNest.ReviewsService.Domain.Entities.Parameters;
using GameNest.ReviewsService.Domain.ValueObjects;

namespace GameNest.ReviewsService.Domain.Interfaces.Services
{
    public interface IMediaService
    {
        Task<PagedList<Media>> GetMediaAsync(MediaParameters parameters, CancellationToken cancellationToken = default);
        Task<Media> GetMediaByIdAsync(string mediaId, CancellationToken cancellationToken = default);
        Task AddMediaAsync(Media media, CancellationToken cancellationToken = default);
        Task UpdateMediaUrlAsync(Guid requesterId, string mediaId, MediaUrl newUrl, CancellationToken cancellationToken = default);
        Task DeleteMediaAsync(Guid requesterId, string mediaId, bool isAdmin, CancellationToken cancellationToken = default);
    }
}