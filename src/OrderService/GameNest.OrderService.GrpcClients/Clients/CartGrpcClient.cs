using GameNest.Grpc.Carts;
using GameNest.OrderService.GrpcClients.Clients.Interfaces;
using Microsoft.Extensions.Logging;

namespace GameNest.OrderService.GrpcClients.Clients
{
    public class CartGrpcClient : ICartGrpcClient
    {
        private readonly CartGrpcService.CartGrpcServiceClient _client;
        private readonly ILogger<CartGrpcClient> _logger;

        public CartGrpcClient(CartGrpcService.CartGrpcServiceClient client, ILogger<CartGrpcClient> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<Cart?> GetCartByUserIdAsync(Guid userId)
        {
            try
            {
                var response = await _client.GetCartByUserIdAsync(
                    new GetCartByUserIdRequest { UserId = userId.ToString() });

                return response.Cart;
            }
            catch (global::Grpc.Core.RpcException ex) when (ex.StatusCode == global::Grpc.Core.StatusCode.NotFound)
            {
                _logger.LogWarning(ex, "Cart for user {UserId} not found.", userId);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching cart for user {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> ClearCartAsync(Guid userId)
        {
            try
            {
                var response = await _client.ClearCartAsync(
                    new ClearCartRequest { UserId = userId.ToString() });

                return response.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while clearing cart for user {UserId}", userId);
                throw;
            }
        }
    }
}
