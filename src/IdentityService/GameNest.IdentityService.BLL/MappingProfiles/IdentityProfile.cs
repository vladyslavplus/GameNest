using AutoMapper;
using GameNest.IdentityService.BLL.DTOs;
using GameNest.IdentityService.Domain.Entities;

namespace GameNest.IdentityService.BLL.MappingProfiles
{
    public class IdentityProfile : Profile
    {
        public IdentityProfile()
        {
            CreateMap<RegisterDto, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

            CreateMap<ApplicationUser, UserDto>()
                .ForMember(dest => dest.Roles, opt => opt.Ignore());

            CreateMap<RefreshToken, RefreshTokenDto>();
        }
    }
}
