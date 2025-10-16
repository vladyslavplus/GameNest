using AutoMapper;
using GameNest.Grpc.OrderItems;
using GameNest.OrderService.BLL.DTOs.OrderItem;

namespace GameNest.OrderService.Grpc.MappingProfiles
{
    public class OrderItemProfile : Profile
    {
        public OrderItemProfile()
        {
            CreateMap<OrderItemDto, OrderItem>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Order_Id.ToString()))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Product_Id.ToString()))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => (double)src.Price))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.Created_At.ToString("yyyy-MM-dd HH:mm:ss")))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.Updated_At.ToString("yyyy-MM-dd HH:mm:ss")));
        }
    }
}