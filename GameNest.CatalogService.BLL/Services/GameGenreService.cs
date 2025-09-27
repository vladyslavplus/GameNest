using AutoMapper;
using GameNest.CatalogService.BLL.DTOs.GameGenres;
using GameNest.CatalogService.BLL.Services.Interfaces;
using GameNest.CatalogService.DAL.Helpers;
using GameNest.CatalogService.DAL.UOW;
using GameNest.CatalogService.Domain.Entities;
using GameNest.CatalogService.Domain.Entities.Parameters;

namespace GameNest.CatalogService.BLL.Services
{
    public class GameGenreService : IGameGenreService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GameGenreService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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

            return _mapper.Map<GameGenreDto>(gameGenre);
        }

        public async Task<GameGenreDto> UpdateGameGenreAsync(Guid id, GameGenreUpdateDto updateDto, CancellationToken cancellationToken = default)
        {
            var gameGenre = await GetGameGenreOrThrowAsync(id, cancellationToken);

            gameGenre.GameId = updateDto.GameId ?? gameGenre.GameId;
            gameGenre.GenreId = updateDto.GenreId ?? gameGenre.GenreId;

            await _unitOfWork.GameGenres.UpdateAsync(gameGenre);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            gameGenre = await _unitOfWork.GameGenres.GetByIdWithReferencesAsync(gameGenre.Id, cancellationToken)
                ?? throw new InvalidOperationException("Failed to load GameGenre after creation.");

            return _mapper.Map<GameGenreDto>(gameGenre);
        }

        public async Task DeleteGameGenreAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await GetGameGenreOrThrowAsync(id, cancellationToken);
            await _unitOfWork.GameGenres.DeleteAsync(id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
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
