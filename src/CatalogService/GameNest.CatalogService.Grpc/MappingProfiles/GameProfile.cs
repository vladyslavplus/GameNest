using AutoMapper;
using GameNest.CatalogService.BLL.DTOs.Games;
using GameNest.Grpc.Games;

namespace GameNest.CatalogService.Grpc.MappingProfiles
{
    public class GameProfile : Profile
    {
        public GameProfile()
        {
            CreateMap<GameDto, Game>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description ?? ""))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => (double)src.Price))
                .ForMember(dest => dest.ReleaseDate, opt => opt.MapFrom(src =>
                    src.ReleaseDate.HasValue ? src.ReleaseDate.Value.ToString("yyyy-MM-dd") : ""))
                .ForMember(dest => dest.PublisherId, opt => opt.MapFrom(src => src.PublisherId.ToString()))
                .ForMember(dest => dest.PublisherName, opt => opt.MapFrom(src => src.PublisherName))
                .ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.Genres))
                .ForMember(dest => dest.Platforms, opt => opt.MapFrom(src => src.Platforms))
                .ForMember(dest => dest.Developers, opt => opt.MapFrom(src => src.Developers));
        }
    }
}