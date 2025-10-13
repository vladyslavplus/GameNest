using AutoMapper;
using GameNest.CatalogService.BLL.DTOs.Games;
using GameNest.CatalogService.BLL.Services.Interfaces;
using GameNest.CatalogService.DAL.Helpers;
using GameNest.CatalogService.DAL.UOW;
using GameNest.CatalogService.Domain.Entities;
using GameNest.CatalogService.Domain.Entities.Parameters;
using GameNest.ServiceDefaults.Redis;
using GameNest.Shared.Helpers;

namespace GameNest.CatalogService.BLL.Services
{
    public class GameService : IGameService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRedisCacheService _cacheService;

        public GameService(IUnitOfWork unitOfWork, IMapper mapper, IRedisCacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public async Task<PagedList<GameDto>> GetGamesPagedAsync(GameParameters parameters, CancellationToken cancellationToken = default)
        {
            string cacheKey = GenerateGamesListCacheKey(parameters);

            var cached = await _cacheService.GetDataAsync<PagedListCacheDto<GameDto>>(cacheKey);
            if (cached != null)
            {
                return new PagedList<GameDto>(
                    cached.Items,
                    cached.TotalCount,
                    cached.PageNumber,
                    cached.PageSize
                );
            }

            var gamesPaged = await _unitOfWork.Games.GetGamesPagedAsync(parameters, cancellationToken: cancellationToken);
            var dtoList = gamesPaged.Select(g => _mapper.Map<GameDto>(g)).ToList();

            var paged = new PagedList<GameDto>(
                dtoList,
                gamesPaged.TotalCount,
                gamesPaged.CurrentPage,
                gamesPaged.PageSize
            );

            var cacheDto = new PagedListCacheDto<GameDto>
            {
                Items = dtoList,
                TotalCount = gamesPaged.TotalCount,
                PageNumber = gamesPaged.CurrentPage,
                PageSize = gamesPaged.PageSize
            };

            await _cacheService.SetDataAsync(cacheKey, cacheDto, TimeSpan.FromMinutes(5));

            return paged;
        }

        public async Task<GameDto?> GetGameByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            string cacheKey = $"game:{id}";

            var cached = await _cacheService.GetDataAsync<GameDto>(cacheKey);
            if (cached != null)
                return cached;

            var game = await _unitOfWork.Games.GetByIdWithDetailsAsync(id, cancellationToken);
            if (game == null)
                return null;

            var dto = _mapper.Map<GameDto>(game);

            await _cacheService.SetDataAsync(cacheKey, dto, TimeSpan.FromMinutes(30));

            return dto;
        }

        public async Task<GameDto> CreateGameAsync(GameCreateDto dto, CancellationToken cancellationToken = default)
        {
            var game = _mapper.Map<Game>(dto);
            await _unitOfWork.Games.AddAsync(game, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var createdDto = _mapper.Map<GameDto>(game);

            await _cacheService.SetDataAsync($"game:{game.Id}", createdDto, TimeSpan.FromMinutes(30));

            await InvalidateGamesListCacheAsync();

            return createdDto;
        }

        public async Task<GameDto> UpdateGameAsync(Guid id, GameUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var game = await GetGameOrThrowAsync(id, cancellationToken);

            game.Title = dto.Title ?? game.Title;
            game.Description = dto.Description ?? game.Description;
            game.Price = dto.Price ?? game.Price;
            game.ReleaseDate = dto.ReleaseDate ?? game.ReleaseDate;
            game.PublisherId = dto.PublisherId ?? game.PublisherId;

            await _unitOfWork.Games.UpdateAsync(game);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var updatedDto = _mapper.Map<GameDto>(game);

            await _cacheService.SetDataAsync($"game:{id}", updatedDto, TimeSpan.FromMinutes(30));
            await InvalidateGamesListCacheAsync();

            return updatedDto;
        }

        public async Task DeleteGameAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await GetGameOrThrowAsync(id, cancellationToken);
            await _unitOfWork.Games.DeleteAsync(id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _cacheService.RemoveDataAsync($"game:{id}");
            await InvalidateGamesListCacheAsync();
        }

        private async Task<Game> GetGameOrThrowAsync(Guid id, CancellationToken cancellationToken)
        {
            var game = await _unitOfWork.Games.GetByIdWithDetailsAsync(id, cancellationToken);
            if (game == null)
                throw new KeyNotFoundException($"Game with id {id} not found.");
            return game;
        }

        private static string GenerateGamesListCacheKey(GameParameters parameters)
        {
            return $"games:page:{parameters.PageNumber}"
                 + $":size:{parameters.PageSize}"
                 + $":order:{parameters.OrderBy ?? "default"}"
                 + $":title:{parameters.Title ?? ""}"
                 + $":minPrice:{parameters.MinPrice?.ToString() ?? ""}"
                 + $":maxPrice:{parameters.MaxPrice?.ToString() ?? ""}"
                 + $":publisher:{parameters.PublisherId?.ToString() ?? ""}";
        }

        private async Task InvalidateGamesListCacheAsync()
        {
            await _cacheService.RemoveByPatternAsync("games:page:*");
        }
    }
}