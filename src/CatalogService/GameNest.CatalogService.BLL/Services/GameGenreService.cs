using AutoMapper;
using GameNest.CatalogService.BLL.DTOs.GameGenres;
using GameNest.CatalogService.BLL.Services.Interfaces;
using GameNest.CatalogService.DAL.Helpers;
using GameNest.CatalogService.DAL.UOW;
using GameNest.CatalogService.Domain.Entities;
using GameNest.CatalogService.Domain.Entities.Parameters;
using GameNest.Shared.Events.GameGenres;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GameNest.CatalogService.BLL.Services
{
    public class GameGenreService : IGameGenreService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<GameGenreService> _logger;

        public GameGenreService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IPublishEndpoint publishEndpoint,
            ILogger<GameGenreService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task<PagedList<GameGenreDto>> GetGameGenresPagedAsync(GameGenreParameters parameters, CancellationToken cancellationToken = default)
        {
            var gameGenresPaged = await _unitOfWork.GameGenres.GetGameGenresPagedAsync(parameters, cancellationToken: cancellationToken);
            var dtoList = gameGenresPaged.Select(g => _mapper.Map<GameGenreDto>(g)).ToList();

            return new PagedList<GameGenreDto>(
                dtoList,
                gameGenresPaged.TotalCount,
                gameGenresPaged.CurrentPage,
                gameGenresPaged.PageSize
            );
        }

        public async Task<GameGenreDto?> GetGameGenreByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var gameGenre = await _unitOfWork.GameGenres.GetByIdWithReferencesAsync(id, cancellationToken);
            return gameGenre == null ? null : _mapper.Map<GameGenreDto>(gameGenre);
        }

        public async Task<GameGenreDto> CreateGameGenreAsync(GameGenreCreateDto createDto, CancellationToken cancellationToken = default)
        {
            var gameGenre = _mapper.Map<GameGenre>(createDto);

            await _unitOfWork.GameGenres.AddAsync(gameGenre, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            gameGenre = await _unitOfWork.GameGenres.GetByIdWithReferencesAsync(gameGenre.Id, cancellationToken)
                   ?? throw new InvalidOperationException("Failed to load GameGenre after creation.");

            var @event = new GameGenreCreatedEvent
            {
                GameGenreId = gameGenre.Id,
                GameId = gameGenre.GameId,
                GenreId = gameGenre.GenreId
            };

            await _publishEndpoint.Publish(@event, cancellationToken);
            _logger.LogInformation("Published GameGenreCreatedEvent for GameId={GameId}, GenreId={GenreId}",
                gameGenre.GameId, gameGenre.GenreId);

            return _mapper.Map<GameGenreDto>(gameGenre);
        }

        public async Task DeleteGameGenreAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var gameGenre = await GetGameGenreOrThrowAsync(id, cancellationToken);
            var gameId = gameGenre.GameId;
            var genreId = gameGenre.GenreId;

            await _unitOfWork.GameGenres.DeleteAsync(id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var @event = new GameGenreDeletedEvent
            {
                GameGenreId = id,
                GameId = gameId,
                GenreId = genreId
            };

            await _publishEndpoint.Publish(@event, cancellationToken);
            _logger.LogInformation("Published GameGenreDeletedEvent for GameId={GameId}, GenreId={GenreId}",
                gameId, genreId);
        }

        private async Task<GameGenre> GetGameGenreOrThrowAsync(Guid id, CancellationToken cancellationToken)
        {
            var gameGenre = await _unitOfWork.GameGenres.GetByIdWithReferencesAsync(id, cancellationToken);
            if (gameGenre == null)
                throw new KeyNotFoundException($"GameGenre with id {id} not found.");
            return gameGenre;
        }
    }
}