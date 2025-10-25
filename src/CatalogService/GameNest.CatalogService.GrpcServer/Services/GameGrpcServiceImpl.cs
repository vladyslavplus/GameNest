using AutoMapper;
using GameNest.CatalogService.BLL.Services.Interfaces;
using GameNest.CatalogService.Domain.Entities.Parameters;
using GameNest.Grpc.Games;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace GameNest.CatalogService.Grpc.Services
{
    public class GameGrpcServiceImpl : GameGrpcService.GameGrpcServiceBase
    {
        private readonly IGameService _gameService;
        private readonly IMapper _mapper;
        private readonly ILogger<GameGrpcServiceImpl> _logger;

        public GameGrpcServiceImpl(
            IGameService gameService,
            IMapper mapper,
            ILogger<GameGrpcServiceImpl> logger)
        {
            _gameService = gameService;
            _mapper = mapper;
            _logger = logger;
        }

        public override async Task<PagedGamesResponse> GetGamesPaged(
            GetGamesRequest request,
            ServerCallContext context)
        {
            try
            {
                var parameters = new GameParameters
                {
                    PageNumber = request.PageNumber > 0 ? request.PageNumber : 1,
                    PageSize = request.PageSize > 0 ? request.PageSize : 10,
                    OrderBy = string.IsNullOrWhiteSpace(request.OrderBy) ? "Id" : request.OrderBy,
                    Title = string.IsNullOrWhiteSpace(request.Title) ? null : request.Title,
                    MinPrice = request.MinPrice > 0 ? (decimal)request.MinPrice : null,
                    MaxPrice = request.MaxPrice > 0 ? (decimal)request.MaxPrice : null,
                    PublisherId = ParseGuid(request.PublisherId)
                };

                var pagedGames = await _gameService.GetGamesPagedAsync(
                    parameters,
                    context.CancellationToken);

                var response = new PagedGamesResponse
                {
                    TotalCount = pagedGames.TotalCount,
                    PageNumber = pagedGames.CurrentPage,
                    PageSize = pagedGames.PageSize
                };

                response.Items.AddRange(pagedGames.Select(g => _mapper.Map<Game>(g)));

                _logger.LogInformation("Returned {Count} games via gRPC", response.Items.Count);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetGamesPaged gRPC call");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public override async Task<GameResponse> GetGameById(
            GetGameByIdRequest request,
            ServerCallContext context)
        {
            try
            {
                if (!Guid.TryParse(request.Id, out var gameId))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid game ID format"));
                }

                var gameDto = await _gameService.GetGameByIdAsync(gameId, context.CancellationToken);

                if (gameDto == null)
                {
                    throw new RpcException(new Status(StatusCode.NotFound, $"Game with ID {gameId} not found"));
                }

                var response = new GameResponse
                {
                    Game = _mapper.Map<Game>(gameDto)
                };

                _logger.LogInformation("Returned game {GameId} via gRPC", gameId);
                return response;
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetGameById gRPC call");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        private static Guid? ParseGuid(string? str)
        {
            return Guid.TryParse(str, out var id) ? id : null;
        }
    }
}