using AutoMapper;
using GameNest.OrderService.Domain.Entities;
using GameNest.OrderService.BLL.DTOs.Product;

namespace GameNest.OrderService.BLL.Mappings
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductDto>();

            CreateMap<ProductCreateDto, Product>();

            CreateMap<ProductUpdateDto, Product>()
                .ForAllMembers(opts =>
                    opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}