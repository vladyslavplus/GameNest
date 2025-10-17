using AutoMapper;
using GameNest.CatalogService.BLL.DTOs.GameDeveloperRoles;
using GameNest.CatalogService.BLL.Services.Interfaces;
using GameNest.CatalogService.DAL.Helpers;
using GameNest.CatalogService.DAL.UOW;
using GameNest.CatalogService.Domain.Entities;
using GameNest.CatalogService.Domain.Entities.Parameters;

namespace GameNest.CatalogService.BLL.Services
{
    public class GameDeveloperRoleService : IGameDeveloperRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GameDeveloperRoleService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedList<GameDeveloperRoleDto>> GetRolesPagedAsync(GameDeveloperRoleParameters parameters, CancellationToken cancellationToken = default)
        {
            var gdrPaged = await _unitOfWork.GameDeveloperRoles.GetRolesPagedAsync(parameters, cancellationToken: cancellationToken);
            var dtoList = gdrPaged.Select(gdr => _mapper.Map<GameDeveloperRoleDto>(gdr)).ToList();

            return new PagedList<GameDeveloperRoleDto>(
                dtoList,
                gdrPaged.TotalCount,
                gdrPaged.CurrentPage,
                gdrPaged.PageSize
            );
        }

        public async Task<GameDeveloperRoleDto?> GetRoleByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var gdr = await _unitOfWork.GameDeveloperRoles.GetByIdWithReferencesAsync(id, cancellationToken);
            return gdr == null ? null : _mapper.Map<GameDeveloperRoleDto>(gdr);
        }

        public async Task<GameDeveloperRoleDto> CreateRoleAsync(GameDeveloperRoleCreateDto createDto, CancellationToken cancellationToken = default)
        {
            var gdr = _mapper.Map<GameDeveloperRole>(createDto);

            await _unitOfWork.GameDeveloperRoles.AddAsync(gdr, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            gdr = await _unitOfWork.GameDeveloperRoles.GetByIdWithReferencesAsync(gdr.Id, cancellationToken)
                  ?? throw new InvalidOperationException("Failed to load GameDeveloperRole after creation.");

            return _mapper.Map<GameDeveloperRoleDto>(gdr);
        }

        public async Task DeleteRoleAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await GetRoleOrThrowAsync(id, cancellationToken);
            await _unitOfWork.GameDeveloperRoles.DeleteAsync(id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private async Task<GameDeveloperRole> GetRoleOrThrowAsync(Guid id, CancellationToken cancellationToken)
        {
            var gdr = await _unitOfWork.GameDeveloperRoles.GetByIdWithReferencesAsync(id, cancellationToken);
            if (gdr == null)
                throw new KeyNotFoundException($"GameDeveloperRole with id {id} not found.");
            return gdr;
        }
    }
}
