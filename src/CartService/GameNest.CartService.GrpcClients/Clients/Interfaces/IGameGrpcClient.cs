using GameNest.Grpc.Games;

namespace GameNest.CartService.GrpcClients.Clients.Interfaces
{
    public interface IGameGrpcClient
    {
        Task<Game?> GetGameByIdAsync(Guid gameId, CancellationToken cancellationToken = default);
    }
}
