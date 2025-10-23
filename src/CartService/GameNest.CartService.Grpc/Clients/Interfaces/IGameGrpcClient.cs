using GameNest.Grpc.Games;

namespace GameNest.CartService.Grpc.Clients.Interfaces
{
    public interface IGameGrpcClient
    {
        Task<Game?> GetGameByIdAsync(Guid gameId, CancellationToken cancellationToken = default);
    }
}
