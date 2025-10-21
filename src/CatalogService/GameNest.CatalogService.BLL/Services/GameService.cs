using AutoMapper;
using GameNest.CatalogService.BLL.Cache.Services.Interfaces;
using GameNest.CatalogService.BLL.DTOs.Games;
using GameNest.CatalogService.BLL.Metrics;
using GameNest.CatalogService.BLL.Services.Interfaces;
using GameNest.CatalogService.DAL.Helpers;
using GameNest.CatalogService.DAL.UOW;
using GameNest.CatalogService.Domain.Entities;
using GameNest.CatalogService.Domain.Entities.Parameters;
using GameNest.ServiceDefaults.Hybrid;
using GameNest.ServiceDefaults.Metrics;
using GameNest.Shared.Helpers;
using System.Diagnostics;

namespace GameNest.CatalogService.BLL.Services
{
    public class GameService : IGameService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHybridCacheService _cacheService;
        private readonly IEntityCacheInvalidationService<Game> _cacheInvalidationService;
        private const string CachePrefix = "game";

        public GameService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHybridCacheService cacheService,
            IEntityCacheInvalidationService<Game> cacheInvalidationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cacheService = cacheService;
            _cacheInvalidationService = cacheInvalidationService;
        }

        public async Task<PagedList<GameDto>> GetGamesPagedAsync(
            GameParameters parameters,
            CancellationToken cancellationToken = default)
        {
            return await MetricRecorder.RecordOperationAsync(
                GameMetrics.OperationLatency,
                TagConstants.Values.List,
                async () =>
                {
                    string cacheKey = GenerateGamesListCacheKey(parameters);

                    var cachedDto = await _cacheService.GetOrSetAsync(
                        cacheKey,
                        async () =>
                        {
                            var gamesPaged = await _unitOfWork.Games.GetGamesPagedAsync(parameters, cancellationToken: cancellationToken);
                            if (gamesPaged is null || !gamesPaged.Any())
                                return null;

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
                        return new PagedList<GameDto>(
                            new List<GameDto>(),
                            0,
                            parameters.PageNumber,
                            parameters.PageSize);
                    }

                    GameMetrics.GamesFetched.Add(1, new TagList { TagConstants.Tags.OperationList });

                    return new PagedList<GameDto>(
                        cachedDto.Items,
                        cachedDto.TotalCount,
                        cachedDto.PageNumber,
                        cachedDto.PageSize);
                });
        }

        public async Task<GameDto?> GetGameByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await MetricRecorder.RecordOperationAsync(
                GameMetrics.OperationLatency,
                TagConstants.Values.GetById,
                async () =>
                {
                    string cacheKey = $"{CachePrefix}:{id}";

                    var result = await _cacheService.GetOrSetAsync(
                        cacheKey,
                        async () =>
                        {
                            var game = await _unitOfWork.Games.GetByIdWithDetailsAsync(id, cancellationToken);
                            return game is null ? null : _mapper.Map<GameDto>(game);
                        },
                        memoryExpiration: TimeSpan.FromMinutes(2),
                        redisExpiration: TimeSpan.FromMinutes(30)
                    );

                    GameMetrics.RecordFetch(TagConstants.Values.GetById, result != null);

                    return result;
                });
        }

        public async Task<GameDto> CreateGameAsync(GameCreateDto gameCreateDto, CancellationToken cancellationToken = default)
        {
            return await MetricRecorder.RecordOperationAsync(
                GameMetrics.OperationLatency,
                TagConstants.Values.Create,
                async () =>
                {
                    var game = _mapper.Map<Game>(gameCreateDto);
                    await _unitOfWork.Games.AddAsync(game, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    var createdDto = _mapper.Map<GameDto>(game);

                    await _cacheInvalidationService.InvalidateAllAsync();

                    string cacheKey = $"{CachePrefix}:{game.Id}";
                    await _cacheService.SetAsync(
                        cacheKey,
                        createdDto,
                        memoryExpiration: TimeSpan.FromMinutes(2),
                        redisExpiration: TimeSpan.FromMinutes(30)
                    );

                    GameMetrics.GamesCreated.Add(1, new TagList { TagConstants.Tags.OperationCreate });

                    return createdDto;
                });
        }

        public async Task<GameDto> UpdateGameAsync(
            Guid id,
            GameUpdateDto updateDto,
            CancellationToken cancellationToken = default)
        {
            return await MetricRecorder.RecordOperationAsync(
                GameMetrics.OperationLatency,
                TagConstants.Values.Update,
                async () =>
                {
                    var game = await GetGameOrThrowAsync(id, cancellationToken);

                    game.Title = updateDto.Title ?? game.Title;
                    game.Description = updateDto.Description ?? game.Description;
                    game.Price = updateDto.Price ?? game.Price;
                    game.ReleaseDate = updateDto.ReleaseDate ?? game.ReleaseDate;
                    game.PublisherId = updateDto.PublisherId ?? game.PublisherId;

                    await _unitOfWork.Games.UpdateAsync(game);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    await _cacheInvalidationService.InvalidateByIdAsync(id);

                    var updatedDto = _mapper.Map<GameDto>(game);
                    string cacheKey = $"{CachePrefix}:{id}";
                    await _cacheService.SetAsync(
                        cacheKey,
                        updatedDto,
                        memoryExpiration: TimeSpan.FromMinutes(2),
                        redisExpiration: TimeSpan.FromMinutes(30)
                    );

                    GameMetrics.GamesUpdated.Add(1, new TagList { TagConstants.Tags.OperationUpdate });

                    return updatedDto;
                });
        }

        public async Task DeleteGameAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await MetricRecorder.RecordOperationAsync(
                GameMetrics.OperationLatency,
                TagConstants.Values.Delete,
                async () =>
                {
                    await GetGameOrThrowAsync(id, cancellationToken);
                    await _unitOfWork.Games.DeleteAsync(id, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    await _cacheInvalidationService.InvalidateByIdAsync(id);

                    GameMetrics.GamesDeleted.Add(1, new TagList { TagConstants.Tags.OperationDelete });
                });
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
    }
}
