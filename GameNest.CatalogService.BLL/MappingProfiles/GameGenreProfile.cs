using AutoMapper;
using GameNest.CatalogService.BLL.DTOs.GameGenres;
using GameNest.CatalogService.Domain.Entities;

namespace GameNest.CatalogService.BLL.MappingProfiles
{
    public class GameGenreProfile : Profile
    {
        public GameGenreProfile()
        {
            CreateMap<GameGenreCreateDto, GameGenre>();
            CreateMap<GameGenreUpdateDto, GameGenre>();

            CreateMap<GameGenre, GameGenreDto>()
                .ForMember(dest => dest.GameTitle, opt => opt.MapFrom(src => src.Game.Title))
                .ForMember(dest => dest.GenreName, opt => opt.MapFrom(src => src.Genre.Name));
        }
    }
}
