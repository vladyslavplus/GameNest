using AutoMapper;
using GameNest.OrderService.Domain.Entities;
using GameNest.OrderService.BLL.DTOs.OrderItem;

namespace GameNest.OrderService.BLL.Mappings
{
    public class OrderItemProfile : Profile
    {
        public OrderItemProfile()
        {
            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.Order_Id, opt => opt.MapFrom(src => src.Order_Id))
                .ForMember(dest => dest.Product_Id, opt => opt.MapFrom(src => src.Product_Id))
                .ForMember(dest => dest.Product_Title, opt => opt.MapFrom(src => src.Product_Title));

            CreateMap<OrderItemDto, OrderItem>()
                .ForMember(dest => dest.Order_Id, opt => opt.MapFrom(src => src.Order_Id))
                .ForMember(dest => dest.Product_Id, opt => opt.MapFrom(src => src.Product_Id))
                .ForMember(dest => dest.Product_Title, opt => opt.MapFrom(src => src.Product_Title));
        }
    }
}