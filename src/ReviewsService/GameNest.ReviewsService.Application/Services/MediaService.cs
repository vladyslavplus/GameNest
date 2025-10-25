using GameNest.ReviewsService.Domain.Common.Helpers;
using GameNest.ReviewsService.Domain.Entities;
using GameNest.ReviewsService.Domain.Entities.Parameters;
using GameNest.ReviewsService.Domain.Exceptions;
using GameNest.ReviewsService.Domain.Interfaces.Repositories;
using GameNest.ReviewsService.Domain.Interfaces.Services;
using GameNest.ReviewsService.Domain.ValueObjects;

namespace GameNest.ReviewsService.Application.Services
{
    public class MediaService : IMediaService
    {
        private readonly IMediaRepository _mediaRepository;

        public MediaService(IMediaRepository mediaRepository)
        {
            _mediaRepository = mediaRepository;
        }

        public async Task<PagedList<Media>> GetMediaAsync(MediaParameters parameters, CancellationToken cancellationToken = default)
        {
            return await _mediaRepository.GetMediaAsync(parameters, cancellationToken);
        }

        public async Task<Media> GetMediaByIdAsync(string mediaId, CancellationToken cancellationToken = default)
        {
            var media = await _mediaRepository.GetByIdAsync(mediaId, cancellationToken);
            if (media == null)
                throw new NotFoundException($"Media with Id '{mediaId}' not found.");
            return media;
        }

        public async Task AddMediaAsync(Media media, CancellationToken cancellationToken = default)
        {
            await _mediaRepository.AddAsync(media, cancellationToken);
        }

        public async Task UpdateMediaUrlAsync(Guid requesterId, string mediaId, MediaUrl newUrl, CancellationToken cancellationToken = default)
        {
            var media = await GetMediaByIdAsync(mediaId, cancellationToken);
            var requesterIdString = requesterId.ToString();

            if (media.CustomerId != requesterIdString)
            {
                throw new ForbiddenException("User is not authorized to update this media.");
            }

            media.UpdateUrl(newUrl, requesterIdString);

            await _mediaRepository.UpdateAsync(media, cancellationToken);
        }

        public async Task DeleteMediaAsync(Guid requesterId, string mediaId, bool isAdmin, CancellationToken cancellationToken = default)
        {
            var media = await GetMediaByIdAsync(mediaId, cancellationToken);

            if (isAdmin)
            {
                await _mediaRepository.DeleteAsync(mediaId, cancellationToken);
                return;
            }

            if (media.CustomerId != requesterId.ToString())
            {
                throw new ForbiddenException("User is not authorized to delete this media.");
            }

            await _mediaRepository.DeleteAsync(mediaId, cancellationToken);
        }
    }
}