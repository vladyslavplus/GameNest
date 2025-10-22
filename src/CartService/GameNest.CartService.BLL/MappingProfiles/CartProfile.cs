using AutoMapper;
using GameNest.CartService.BLL.DTOs;
using GameNest.CartService.Domain.Entities;

namespace GameNest.CartService.BLL.MappingProfiles
{
    public class CartProfile : Profile
    {
        public CartProfile()
        {
            CreateMap<ShoppingCart, ShoppingCartDto>();
            CreateMap<ShoppingCartItem, ShoppingCartItemDto>();

            CreateMap<ShoppingCartItemDto, ShoppingCartItem>();

            CreateMap<CartItemAddDto, ShoppingCartItem>();
        }
    }
}
