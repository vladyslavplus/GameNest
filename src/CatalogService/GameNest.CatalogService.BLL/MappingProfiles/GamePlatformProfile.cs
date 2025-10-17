using AutoMapper;
using GameNest.CatalogService.BLL.DTOs.GamePlatforms;
using GameNest.CatalogService.Domain.Entities;

namespace GameNest.CatalogService.BLL.MappingProfiles
{
    public class GamePlatformProfile : Profile
    {
        public GamePlatformProfile()
        {
            CreateMap<GamePlatformCreateDto, GamePlatform>();

            CreateMap<GamePlatform, GamePlatformDto>()
                .ForMember(dest => dest.GameTitle, opt => opt.MapFrom(src => src.Game.Title))
                .ForMember(dest => dest.PlatformName, opt => opt.MapFrom(src => src.Platform.Name));
        }
    }
}
