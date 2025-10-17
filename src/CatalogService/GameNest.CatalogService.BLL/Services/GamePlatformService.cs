using AutoMapper;
using GameNest.CatalogService.BLL.DTOs.GamePlatforms;
using GameNest.CatalogService.BLL.Services.Interfaces;
using GameNest.CatalogService.DAL.Helpers;
using GameNest.CatalogService.DAL.UOW;
using GameNest.CatalogService.Domain.Entities;
using GameNest.CatalogService.Domain.Entities.Parameters;
using GameNest.Shared.Events.GamePlatforms;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GameNest.CatalogService.BLL.Services
{
    public class GamePlatformService : IGamePlatformService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<GamePlatformService> _logger;

        public GamePlatformService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IPublishEndpoint publishEndpoint,
            ILogger<GamePlatformService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task<PagedList<GamePlatformDto>> GetGamePlatformsPagedAsync(GamePlatformParameters parameters, CancellationToken cancellationToken = default)
        {
            var gamePlatformsPaged = await _unitOfWork.GamePlatforms.GetGamePlatformsPagedAsync(parameters, cancellationToken: cancellationToken);
            var dtoList = gamePlatformsPaged.Select(gp => _mapper.Map<GamePlatformDto>(gp)).ToList();

            return new PagedList<GamePlatformDto>(
                dtoList,
                gamePlatformsPaged.TotalCount,
                gamePlatformsPaged.CurrentPage,
                gamePlatformsPaged.PageSize
            );
        }

        public async Task<GamePlatformDto?> GetGamePlatformByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var gamePlatform = await _unitOfWork.GamePlatforms.GetByIdWithReferencesAsync(id, cancellationToken);
            return gamePlatform == null ? null : _mapper.Map<GamePlatformDto>(gamePlatform);
        }

        public async Task<GamePlatformDto> CreateGamePlatformAsync(GamePlatformCreateDto createDto, CancellationToken cancellationToken = default)
        {
            var gamePlatform = _mapper.Map<GamePlatform>(createDto);

            await _unitOfWork.GamePlatforms.AddAsync(gamePlatform, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            gamePlatform = await _unitOfWork.GamePlatforms.GetByIdWithReferencesAsync(gamePlatform.Id, cancellationToken)
                           ?? throw new InvalidOperationException("Failed to load GamePlatform after creation.");

            var @event = new GamePlatformCreatedEvent
            {
                GamePlatformId = gamePlatform.Id,
                GameId = gamePlatform.GameId,
                PlatformId = gamePlatform.PlatformId
            };

            await _publishEndpoint.Publish(@event, cancellationToken);
            _logger.LogInformation("Published GamePlatformCreatedEvent for GameId={GameId}, PlatformId={PlatformId}",
                gamePlatform.GameId, gamePlatform.PlatformId);

            return _mapper.Map<GamePlatformDto>(gamePlatform);
        }

        public async Task DeleteGamePlatformAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var gamePlatform = await GetGamePlatformOrThrowAsync(id, cancellationToken);
            var gameId = gamePlatform.GameId;
            var platformId = gamePlatform.PlatformId;

            await _unitOfWork.GamePlatforms.DeleteAsync(id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var @event = new GamePlatformDeletedEvent
            {
                GamePlatformId = id,
                GameId = gameId,
                PlatformId = platformId
            };

            await _publishEndpoint.Publish(@event, cancellationToken);
            _logger.LogInformation("Published GamePlatformDeletedEvent for GameId={GameId}, PlatformId={PlatformId}",
                gameId, platformId);
        }

        private async Task<GamePlatform> GetGamePlatformOrThrowAsync(Guid id, CancellationToken cancellationToken)
        {
            var gamePlatform = await _unitOfWork.GamePlatforms.GetByIdWithReferencesAsync(id, cancellationToken);
            if (gamePlatform == null)
                throw new KeyNotFoundException($"GamePlatform with id {id} not found.");
            return gamePlatform;
        }
    }
}
