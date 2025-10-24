using GameNest.CartService.GrpcClients.Clients.Interfaces;
using GameNest.Grpc.Games;
using Microsoft.Extensions.Logging;

namespace GameNest.CartService.GrpcClients.Clients
{
    public class GameGrpcClient : IGameGrpcClient
    {
        private readonly GameGrpcService.GameGrpcServiceClient _client;
        private readonly ILogger<GameGrpcClient> _logger;

        public GameGrpcClient(GameGrpcService.GameGrpcServiceClient client, ILogger<GameGrpcClient> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<Game?> GetGameByIdAsync(Guid gameId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _client.GetGameByIdAsync(new GetGameByIdRequest
                {
                    Id = gameId.ToString()
                }, cancellationToken: cancellationToken);

                return response.Game;
            }
            catch (global::Grpc.Core.RpcException ex) when (ex.StatusCode == global::Grpc.Core.StatusCode.NotFound)
            {
                _logger.LogWarning(ex, "Game with ID {GameId} not found.", gameId);
                return null;
            }
        }
    }
}
