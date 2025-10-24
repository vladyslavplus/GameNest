using AutoMapper;
using GameNest.CartService.BLL.DTOs;
using GameNest.Grpc.Carts;

namespace GameNest.CartService.GrpcServer.MappingProfiles
{
    public class CartGrpcProfile : Profile
    {
        public CartGrpcProfile()
        {
            CreateMap<ShoppingCartDto, Cart>()
                        .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId.ToString()))
                        .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            CreateMap<ShoppingCartItemDto, CartItem>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId.ToString()))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => (double)src.Price))
                .ForMember(dest => dest.ProductTitle, opt => opt.MapFrom(src => src.ProductTitle));
        }
    }
}
