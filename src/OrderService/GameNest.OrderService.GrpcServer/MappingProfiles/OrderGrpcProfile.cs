using AutoMapper;
using GameNest.Grpc.Orders;
using GameNest.OrderService.BLL.DTOs.Order;

namespace GameNest.OrderService.GrpcServer.MappingProfiles
{
    public class OrderGrpcProfile : Profile
    {
        public OrderGrpcProfile()
        {
            CreateMap<OrderDto, Order>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Customer_Id.ToString()))
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.Order_Date.ToString("yyyy-MM-dd HH:mm:ss")))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => (double)src.Total_Amount))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.Created_At.ToString("yyyy-MM-dd HH:mm:ss")))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
                .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Street))
                .ForMember(dest => dest.ZipCode, opt => opt.MapFrom(src => src.ZipCode))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
                .ReverseMap();
        }
    }
}