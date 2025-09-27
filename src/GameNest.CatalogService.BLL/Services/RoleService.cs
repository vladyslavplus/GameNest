using AutoMapper;
using GameNest.CatalogService.BLL.DTOs.Roles;
using GameNest.CatalogService.BLL.Services.Interfaces;
using GameNest.CatalogService.DAL.Helpers;
using GameNest.CatalogService.DAL.UOW;
using GameNest.CatalogService.Domain.Entities;
using GameNest.CatalogService.Domain.Entities.Parameters;

namespace GameNest.CatalogService.BLL.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RoleService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedList<RoleDto>> GetRolesPagedAsync(RoleParameters parameters, CancellationToken cancellationToken = default)
        {
            var rolesPaged = await _unitOfWork.Roles.GetRolesPagedAsync(parameters, cancellationToken: cancellationToken);
            var dtoList = rolesPaged.Select(r => _mapper.Map<RoleDto>(r)).ToList();

            return new PagedList<RoleDto>(
                dtoList,
                rolesPaged.TotalCount,
                rolesPaged.CurrentPage,
                rolesPaged.PageSize
            );
        }

        public async Task<RoleDto?> GetRoleByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var role = await _unitOfWork.Roles.GetByIdAsync(id, cancellationToken: cancellationToken);
            return role == null ? null : _mapper.Map<RoleDto>(role);
        }

        public async Task<RoleDto> CreateRoleAsync(RoleCreateDto createDto, CancellationToken cancellationToken = default)
        {
            var role = _mapper.Map<Role>(createDto);
            await _unitOfWork.Roles.AddAsync(role, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<RoleDto>(role);
        }

        public async Task<RoleDto> UpdateRoleAsync(Guid id, RoleUpdateDto updateDto, CancellationToken cancellationToken = default)
        {
            var role = await GetRoleOrThrowAsync(id, cancellationToken);

            role.Name = updateDto.Name ?? role.Name;

            await _unitOfWork.Roles.UpdateAsync(role);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<RoleDto>(role);
        }

        public async Task DeleteRoleAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var role = await GetRoleOrThrowAsync(id, cancellationToken);

            await _unitOfWork.Roles.DeleteAsync(role.Id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private async Task<Role> GetRoleOrThrowAsync(Guid id, CancellationToken cancellationToken)
        {
            var role = await _unitOfWork.Roles.GetByIdAsync(id, cancellationToken: cancellationToken);
            if (role == null)
                throw new KeyNotFoundException($"Role with id {id} not found.");
            return role;
        }
    }
}
