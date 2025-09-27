using AutoMapper;
using GameNest.CatalogService.BLL.DTOs.Games;
using GameNest.CatalogService.Domain.Entities;

namespace GameNest.CatalogService.BLL.MappingProfiles
{
    public class GameProfile : Profile
    {
        public GameProfile()
        {
            CreateMap<Game, GameDto>()
                .ForMember(dest => dest.PublisherName, opt => opt.MapFrom(src => src.Publisher != null ? src.Publisher.Name : null))
                .ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.GameGenres.Select(g => g.Genre.Name)))
                .ForMember(dest => dest.Platforms, opt => opt.MapFrom(src => src.GamePlatforms.Select(g => g.Platform.Name)))
                .ForMember(dest => dest.Developers, opt => opt.MapFrom(src => src.GameDeveloperRoles.Select(g => g.Developer.FullName)));

            CreateMap<GameCreateDto, Game>()
                .ForMember(dest => dest.GameGenres, opt => opt.Ignore())
                .ForMember(dest => dest.GamePlatforms, opt => opt.Ignore())
                .ForMember(dest => dest.GameDeveloperRoles, opt => opt.Ignore())
                .ForMember(dest => dest.Publisher, opt => opt.Ignore());

            CreateMap<GameUpdateDto, Game>()
                .ForMember(dest => dest.GameGenres, opt => opt.Ignore())
                .ForMember(dest => dest.GamePlatforms, opt => opt.Ignore())
                .ForMember(dest => dest.GameDeveloperRoles, opt => opt.Ignore())
                .ForMember(dest => dest.Publisher, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}