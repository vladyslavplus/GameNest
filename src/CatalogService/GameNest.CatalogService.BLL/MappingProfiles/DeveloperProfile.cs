using AutoMapper;
using GameNest.CatalogService.BLL.DTOs.Developers;
using GameNest.CatalogService.Domain.Entities;

namespace GameNest.CatalogService.BLL.MappingProfiles
{
    public class DeveloperProfile : Profile
    {
        public DeveloperProfile()
        {
            CreateMap<DeveloperCreateDto, Developer>();

            CreateMap<DeveloperUpdateDto, Developer>();

            CreateMap<Developer, DeveloperDto>()
                .ForMember(dest => dest.GameRoles, opt => opt.MapFrom(src =>
                    src.GameDeveloperRoles.Select(gdr => new DeveloperGameRoleDto
                    {
                        GameId = gdr.GameId,
                        GameName = gdr.Game.Title,
                        RoleName = gdr.Role.Name,
                        Seniority = gdr.Seniority
                    })
                ));
        }
    }
}
