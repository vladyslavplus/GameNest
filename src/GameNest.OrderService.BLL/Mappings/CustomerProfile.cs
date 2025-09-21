using AutoMapper;
using GameNest.OrderService.BLL.DTOs.Customer;
using GameNest.OrderService.Domain.Entities;

namespace GameNest.OrderService.BLL.Mappings
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            CreateMap<Customer, CustomerDto>();

            CreateMap<CustomerCreateDto, Customer>();

            CreateMap<CustomerUpdateDto, Customer>()
                .ForAllMembers(opts => 
                    opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
