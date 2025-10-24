using AutoMapper;
using GameNest.OrderService.Domain.Entities;
using GameNest.OrderService.BLL.DTOs.Order;

namespace GameNest.OrderService.BLL.Mappings
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.Customer_Id, opt => opt.MapFrom(src => src.Customer_Id))
                .ForMember(dest => dest.Order_Date, opt => opt.MapFrom(src => src.Order_Date))
                .ForMember(dest => dest.Total_Amount, opt => opt.MapFrom(src => src.Total_Amount))
                .ForMember(dest => dest.Created_At, opt => opt.MapFrom(src => src.Created_At))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            CreateMap<OrderDto, Order>()
                .ForMember(dest => dest.Customer_Id, opt => opt.MapFrom(src => src.Customer_Id))
                .ForMember(dest => dest.Order_Date, opt => opt.MapFrom(src => src.Order_Date))
                .ForMember(dest => dest.Total_Amount, opt => opt.MapFrom(src => src.Total_Amount))
                .ForMember(dest => dest.Created_At, opt => opt.MapFrom(src => src.Created_At))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            CreateMap<OrderCreateDto, Order>();

            CreateMap<OrderUpdateDto, Order>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}