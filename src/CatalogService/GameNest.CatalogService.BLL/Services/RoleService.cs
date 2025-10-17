using AutoMapper;
using GameNest.CatalogService.BLL.DTOs.Roles;
using GameNest.CatalogService.BLL.Services.Interfaces;
using GameNest.CatalogService.DAL.Helpers;
using GameNest.CatalogService.DAL.UOW;
using GameNest.CatalogService.Domain.Entities;
using GameNest.CatalogService.Domain.Entities.Parameters;
using GameNest.Shared.Events.Roles;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GameNest.CatalogService.BLL.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<RoleService> _logger;

        public RoleService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IPublishEndpoint publishEndpoint,
            ILogger<RoleService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
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

            var oldName = role.Name;
            role.Name = updateDto.Name ?? role.Name;

            await _unitOfWork.Roles.UpdateAsync(role);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var @event = new RoleUpdatedEvent
            {
                RoleId = role.Id,
                OldName = oldName,
                NewName = role.Name
            };

            await _publishEndpoint.Publish(@event, cancellationToken);
            _logger.LogInformation(
                "Published RoleUpdatedEvent for Role {RoleId}: {OldName} -> {NewName}",
                role.Id, oldName, role.Name
            );

            return _mapper.Map<RoleDto>(role);
        }

        public async Task DeleteRoleAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var role = await GetRoleOrThrowAsync(id, cancellationToken);
            var roleName = role.Name;

            await _unitOfWork.Roles.DeleteAsync(role.Id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var @event = new RoleDeletedEvent
            {
                RoleId = id,
                RoleName = roleName
            };

            await _publishEndpoint.Publish(@event, cancellationToken);
            _logger.LogInformation(
                "Published RoleDeletedEvent for Role {RoleId}: {RoleName}",
                id, roleName
            );
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
