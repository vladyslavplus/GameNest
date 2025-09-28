using AutoMapper;
using GameNest.OrderService.Domain.Entities;
using GameNest.OrderService.BLL.DTOs.PaymentRecord;

namespace GameNest.OrderService.BLL.Mappings
{
    public class PaymentRecordProfile : Profile
    {
        public PaymentRecordProfile()
        {
            CreateMap<PaymentRecord, PaymentRecordDto>();

            CreateMap<PaymentRecordCreateDto, PaymentRecord>();

            CreateMap<PaymentRecordUpdateDto, PaymentRecord>()
                .ForAllMembers(opts =>
                    opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}