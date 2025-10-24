using GameNest.Grpc.Carts;

namespace GameNest.OrderService.GrpcClients.Clients.Interfaces
{
    public interface ICartGrpcClient
    {
        Task<Cart?> GetCartByUserIdAsync(Guid userId);
        Task<bool> ClearCartAsync(Guid userId);
    }
}
