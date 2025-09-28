using AutoMapper;
using GameNest.CatalogService.BLL.DTOs.GameDeveloperRoles;
using GameNest.CatalogService.Domain.Entities;

namespace GameNest.CatalogService.BLL.MappingProfiles
{
    public class GameDeveloperRoleProfile : Profile
    {
        public GameDeveloperRoleProfile()
        {
            CreateMap<GameDeveloperRoleCreateDto, GameDeveloperRole>();
            CreateMap<GameDeveloperRoleUpdateDto, GameDeveloperRole>();

            CreateMap<GameDeveloperRole, GameDeveloperRoleDto>()
                .ForMember(dest => dest.GameTitle, opt => opt.MapFrom(src => src.Game.Title))
                .ForMember(dest => dest.DeveloperFullName, opt => opt.MapFrom(src => src.Developer.FullName))
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name));
        }
    }
}
