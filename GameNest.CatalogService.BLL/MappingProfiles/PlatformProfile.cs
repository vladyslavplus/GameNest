using AutoMapper;
using GameNest.CatalogService.BLL.DTOs.Platforms;
using GameNest.CatalogService.Domain.Entities;

namespace GameNest.CatalogService.BLL.MappingProfiles
{
    public class PlatformProfile : Profile
    {
        public PlatformProfile()
        {
            CreateMap<PlatformCreateDto, Platform>();
            CreateMap<PlatformUpdateDto, Platform>();
            CreateMap<Platform, PlatformDto>();
        }
    }
}
