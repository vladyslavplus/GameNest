using GameNest.Grpc.Games;
using Grpc.Core;

namespace GameNest.AggregatorService.Clients
{
    public class CatalogGrpcClient
    {
        private readonly GameGrpcService.GameGrpcServiceClient _client;
        private readonly ILogger<CatalogGrpcClient> _logger;

        public CatalogGrpcClient(
            GameGrpcService.GameGrpcServiceClient client,
            ILogger<CatalogGrpcClient> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<IEnumerable<Game>?> GetGamesAsync(CancellationToken ct)
        {
            try
            {
                var request = new GetGamesRequest
                {
                    PageNumber = 1,
                    PageSize = 1000 
                };

                var response = await _client.GetGamesPagedAsync(request, cancellationToken: ct);
                return response.Items;
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "gRPC error while fetching games from CatalogService");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching games from CatalogService");
                return null;
            }
        }

        public async Task<Game?> GetGameByIdAsync(Guid gameId, CancellationToken ct)
        {
            try
            {
                var request = new GetGameByIdRequest
                {
                    Id = gameId.ToString()
                };

                var response = await _client.GetGameByIdAsync(request, cancellationToken: ct);
                return response.Game;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                _logger.LogWarning(ex, "Game {GameId} not found", gameId);
                return null;
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "gRPC error while fetching game {GameId}", gameId);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching game {GameId}", gameId);
                return null;
            }
        }
    }
}
