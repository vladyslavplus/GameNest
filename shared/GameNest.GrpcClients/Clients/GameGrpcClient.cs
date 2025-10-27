using GameNest.Grpc.Games;
using GameNest.GrpcClients.Interfaces;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace GameNest.GrpcClients.Clients
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
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                _logger.LogWarning(ex, "Game with ID {GameId} not found.", gameId);
                return null;
            }
        }
    }
}
