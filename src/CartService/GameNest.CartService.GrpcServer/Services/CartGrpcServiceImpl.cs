using AutoMapper;
using GameNest.CartService.BLL.Interfaces;
using GameNest.Grpc.Carts;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace GameNest.CartService.GrpcServer.Services
{
    public class CartGrpcServiceImpl : CartGrpcService.CartGrpcServiceBase
    {
        private readonly ICartService _cartService;
        private readonly IMapper _mapper;
        private readonly ILogger<CartGrpcServiceImpl> _logger;

        public CartGrpcServiceImpl(
            ICartService cartService,
            IMapper mapper,
            ILogger<CartGrpcServiceImpl> logger)
        {
            _cartService = cartService;
            _mapper = mapper;
            _logger = logger;
        }

        public override async Task<CartResponse> GetCartByUserId(
            GetCartByUserIdRequest request,
            ServerCallContext context)
        {
            try
            {
                if (!Guid.TryParse(request.UserId, out var userId))
                {
                    _logger.LogWarning("Invalid user ID format received in gRPC request: {UserId}", request.UserId);
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid user ID format"));
                }

                _logger.LogInformation("gRPC: Attempting to get cart for user {UserId}", userId);

                var cartDto = await _cartService.GetCartAsync(userId);

                var grpcCart = _mapper.Map<Cart>(cartDto);

                return new CartResponse { Cart = grpcCart };
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetCartByUserId gRPC call for user {UserId}", request.UserId);
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public override async Task<ClearCartResponse> ClearCart(
            ClearCartRequest request,
            ServerCallContext context)
        {
            try
            {
                if (!Guid.TryParse(request.UserId, out var userId))
                {
                    _logger.LogWarning("Invalid user ID format received in gRPC request: {UserId}", request.UserId);
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid user ID format"));
                }

                _logger.LogInformation("gRPC: Attempting to clear cart for user {UserId}", userId);

                await _cartService.ClearCartAsync(userId);

                return new ClearCartResponse { Success = true };
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ClearCart gRPC call for user {UserId}", request.UserId);
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }
    }
}
