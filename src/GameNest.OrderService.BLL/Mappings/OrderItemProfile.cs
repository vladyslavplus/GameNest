using AutoMapper;
using GameNest.OrderService.Domain.Entities;
using GameNest.OrderService.BLL.DTOs.OrderItem;

namespace GameNest.OrderService.BLL.Mappings
{
    public class OrderItemProfile : Profile
    {
        public OrderItemProfile()
        {
            CreateMap<OrderItem, OrderItemDto>();

            CreateMap<OrderItemCreateDto, OrderItem>();

            CreateMap<OrderItemUpdateDto, OrderItem>()
                .ForAllMembers(opts =>
                    opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}