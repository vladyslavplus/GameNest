using GameNest.GrpcClients.Interfaces;
using GameNest.ReviewsService.Domain.Common.Helpers;
using GameNest.ReviewsService.Domain.Entities;
using GameNest.ReviewsService.Domain.Entities.Parameters;
using GameNest.ReviewsService.Domain.Exceptions;
using GameNest.ReviewsService.Domain.Interfaces.Repositories;
using GameNest.ReviewsService.Domain.Interfaces.Services;
using GameNest.ReviewsService.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace GameNest.ReviewsService.Application.Services
{
    public class MediaService : IMediaService
    {
        private readonly IMediaRepository _mediaRepository;
        private readonly IGameGrpcClient _gameClient;
        private readonly ILogger<MediaService> _logger;

        public MediaService(IMediaRepository mediaRepository, IGameGrpcClient gameClient, ILogger<MediaService> logger)
        {
            _mediaRepository = mediaRepository;
            _gameClient = gameClient;
            _logger = logger;
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
            var game = await _gameClient.GetGameByIdAsync(Guid.Parse(media.GameId), cancellationToken);
            if (game == null)
            {
                _logger.LogWarning("Attempted to add media for non-existent game {GameId}.", media.GameId);
                throw new NotFoundException($"Game with Id '{media.GameId}' not found in CatalogService.");
            }

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