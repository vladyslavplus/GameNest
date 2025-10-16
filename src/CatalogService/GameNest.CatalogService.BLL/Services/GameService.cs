using AutoMapper;
using GameNest.CatalogService.BLL.Cache.Services;
using GameNest.CatalogService.BLL.DTOs.Games;
using GameNest.CatalogService.BLL.Services.Interfaces;
using GameNest.CatalogService.DAL.Helpers;
using GameNest.CatalogService.DAL.UOW;
using GameNest.CatalogService.Domain.Entities;
using GameNest.CatalogService.Domain.Entities.Parameters;
using GameNest.ServiceDefaults.Hybrid;
using GameNest.ServiceDefaults.Memory;
using GameNest.ServiceDefaults.Redis;
using GameNest.Shared.Helpers;

namespace GameNest.CatalogService.BLL.Services
{
    public class GameService : IGameService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHybridCacheService _cacheService;
        private readonly IGameCacheInvalidationService _cacheInvalidationService;

        public GameService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHybridCacheService cacheService,
            IGameCacheInvalidationService cacheInvalidationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cacheService = cacheService;
            _cacheInvalidationService = cacheInvalidationService;
        }

        public async Task<PagedList<GameDto>> GetGamesPagedAsync(GameParameters parameters, CancellationToken cancellationToken = default)
        {
            string cacheKey = GenerateGamesListCacheKey(parameters);

            var cachedDto = await _cacheService.GetOrSetAsync(
                cacheKey,
                async () =>
                {
                    var gamesPaged = await _unitOfWork.Games.GetGamesPagedAsync(parameters, cancellationToken: cancellationToken);
                    if (gamesPaged is null || !gamesPaged.Any())
                    {
                        return null; 
                    }
                    var dtoList = gamesPaged.Select(g => _mapper.Map<GameDto>(g)).ToList();
                    return new PagedListCacheDto<GameDto>
                    {
                        Items = dtoList,
                        TotalCount = gamesPaged.TotalCount,
                        PageNumber = gamesPaged.CurrentPage,
                        PageSize = gamesPaged.PageSize
                    };
                },
                memoryExpiration: TimeSpan.FromSeconds(30),
                redisExpiration: TimeSpan.FromMinutes(5)
            );

            if (cachedDto is null)
            {
                return new PagedList<GameDto>(new List<GameDto>(), 0, parameters.PageNumber, parameters.PageSize);
            }

            return new PagedList<GameDto>(
                cachedDto.Items,
                cachedDto.TotalCount,
                cachedDto.PageNumber,
                cachedDto.PageSize
            );
        }

        public async Task<GameDto?> GetGameByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            string cacheKey = $"game:{id}";

            return await _cacheService.GetOrSetAsync(
                cacheKey,
                async () => {
                    var game = await _unitOfWork.Games.GetByIdWithDetailsAsync(id, cancellationToken);
                    return game is null ? null : _mapper.Map<GameDto>(game);
                },
                memoryExpiration: TimeSpan.FromMinutes(2),
                redisExpiration: TimeSpan.FromMinutes(30)
            );
        }

        public async Task<GameDto> CreateGameAsync(GameCreateDto dto, CancellationToken cancellationToken = default)
        {
            var game = _mapper.Map<Game>(dto);
            await _unitOfWork.Games.AddAsync(game, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var createdDto = _mapper.Map<GameDto>(game);

            await _cacheService.SetAsync($"game:{game.Id}", createdDto,
                memoryExpiration: TimeSpan.FromMinutes(2),
                redisExpiration: TimeSpan.FromMinutes(30));

            await _cacheInvalidationService.InvalidateAllGamesAsync();

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

            await _cacheInvalidationService.InvalidateGameAsync(id);

            return _mapper.Map<GameDto>(game);
        }

        public async Task DeleteGameAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await GetGameOrThrowAsync(id, cancellationToken);
            await _unitOfWork.Games.DeleteAsync(id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _cacheInvalidationService.InvalidateGameAsync(id);
        }

        private async Task<Game> GetGameOrThrowAsync(Guid id, CancellationToken cancellationToken)
        {
            var game = await _unitOfWork.Games.GetByIdWithDetailsAsync(id, cancellationToken);
            if (game == null)
                throw new KeyNotFoundException($"Game with id {id} not found.");
            return game;
        }

        public static string GenerateGamesListCacheKey(GameParameters parameters)
        {
            return $"games:page:{parameters.PageNumber}"
                 + $":size:{parameters.PageSize}"
                 + $":order:{parameters.OrderBy ?? "Id"}"
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