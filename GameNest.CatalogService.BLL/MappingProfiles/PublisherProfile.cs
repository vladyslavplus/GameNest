using AutoMapper;
using GameNest.CatalogService.BLL.DTOs.Publishers;
using GameNest.CatalogService.Domain.Entities;

namespace GameNest.CatalogService.BLL.MappingProfiles
{
    public class PublisherProfile : Profile
    {
        public PublisherProfile()
        {
            CreateMap<PublisherCreateDto, Publisher>();
            CreateMap<PublisherUpdateDto, Publisher>();

            CreateMap<Publisher, PublisherDto>()
                .ForMember(dest => dest.Games, opt => opt.MapFrom(src =>
                    src.Games.Select(g => g.Title).ToList()
                ));
        }
    }
}
