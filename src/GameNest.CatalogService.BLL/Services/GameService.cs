using AutoMapper;
using GameNest.CatalogService.BLL.DTOs.Games;
using GameNest.CatalogService.BLL.Services.Interfaces;
using GameNest.CatalogService.DAL.Helpers;
using GameNest.CatalogService.DAL.UOW;
using GameNest.CatalogService.Domain.Entities;
using GameNest.CatalogService.Domain.Entities.Parameters;

namespace GameNest.CatalogService.BLL.Services
{
    public class GameService : IGameService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GameService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedList<GameDto>> GetGamesPagedAsync(GameParameters parameters, CancellationToken cancellationToken = default)
        {
            var gamesPaged = await _unitOfWork.Games.GetGamesPagedAsync(parameters, cancellationToken: cancellationToken);
            var dtoList = gamesPaged.Select(g => _mapper.Map<GameDto>(g)).ToList();

            return new PagedList<GameDto>(
                dtoList,
                gamesPaged.TotalCount,
                gamesPaged.CurrentPage,
                gamesPaged.PageSize
            );
        }

        public async Task<GameDto?> GetGameByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var game = await _unitOfWork.Games.GetByIdWithDetailsAsync(id, cancellationToken: cancellationToken);
            return game == null ? null : _mapper.Map<GameDto>(game);
        }

        public async Task<GameDto> CreateGameAsync(GameCreateDto gameCreateDto, CancellationToken cancellationToken = default)
        {
            var game = _mapper.Map<Game>(gameCreateDto);
            await _unitOfWork.Games.AddAsync(game, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<GameDto>(game);
        }

        public async Task<GameDto> UpdateGameAsync(Guid id, GameUpdateDto updateDto, CancellationToken cancellationToken = default)
        {
            var game = await GetGameOrThrowAsync(id, cancellationToken);

            game.Title = updateDto.Title ?? game.Title;
            game.Description = updateDto.Description ?? game.Description;
            game.ReleaseDate = updateDto.ReleaseDate ?? game.ReleaseDate;
            game.Price = updateDto.Price ?? game.Price;
            game.PublisherId = updateDto.PublisherId ?? game.PublisherId;

            await _unitOfWork.Games.UpdateAsync(game);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<GameDto>(game);
        }

        public async Task DeleteGameAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await GetGameOrThrowAsync(id, cancellationToken);
            await _unitOfWork.Games.DeleteAsync(id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private async Task<Game> GetGameOrThrowAsync(Guid id, CancellationToken cancellationToken)
        {
            var game = await _unitOfWork.Games.GetByIdWithDetailsAsync(id, cancellationToken);
            if (game == null)
                throw new KeyNotFoundException($"Game with id {id} not found.");
            return game;
        }
    }
}