using AutoMapper;
using GameNest.Grpc.OrderItems;
using GameNest.OrderService.BLL.DTOs.OrderItem;

namespace GameNest.OrderService.GrpcServer.MappingProfiles
{
    public class OrderItemGrpcProfile : Profile
    {
        public OrderItemGrpcProfile()
        {
            CreateMap<OrderItemDto, OrderItem>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Order_Id.ToString()))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Product_Id.ToString()))
                .ForMember(dest => dest.ProductTitle, opt => opt.MapFrom(src => src.Product_Title))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => (double)src.Price))
                .ReverseMap();
        }
    }
}