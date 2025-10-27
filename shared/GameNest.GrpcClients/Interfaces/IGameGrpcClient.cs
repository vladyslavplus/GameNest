using GameNest.Grpc.Games;

namespace GameNest.GrpcClients.Interfaces
{
    public interface IGameGrpcClient
    {
        Task<Game?> GetGameByIdAsync(Guid gameId, CancellationToken cancellationToken = default);
    }
}
