using AutoMapper;
using GameNest.CatalogService.BLL.DTOs.GameDeveloperRoles;
using GameNest.CatalogService.BLL.Services.Interfaces;
using GameNest.CatalogService.DAL.Helpers;
using GameNest.CatalogService.DAL.UOW;
using GameNest.CatalogService.Domain.Entities;
using GameNest.CatalogService.Domain.Entities.Parameters;
using GameNest.Shared.Events.GameDeveloperRoles;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GameNest.CatalogService.BLL.Services
{
    public class GameDeveloperRoleService : IGameDeveloperRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<GameDeveloperRoleService> _logger;

        public GameDeveloperRoleService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IPublishEndpoint publishEndpoint,
            ILogger<GameDeveloperRoleService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
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

            var @event = new GameDeveloperRoleCreatedEvent
            {
                GameDeveloperRoleId = gdr.Id,
                GameId = gdr.GameId,
                DeveloperId = gdr.DeveloperId,
                RoleId = gdr.RoleId
            };

            await _publishEndpoint.Publish(@event, cancellationToken);
            _logger.LogInformation(
                "Published GameDeveloperRoleCreatedEvent for GameId={GameId}, DeveloperId={DeveloperId}, RoleId={RoleId}",
                gdr.GameId, gdr.DeveloperId, gdr.RoleId
            );

            return _mapper.Map<GameDeveloperRoleDto>(gdr);
        }

        public async Task DeleteRoleAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var gdr = await GetRoleOrThrowAsync(id, cancellationToken);
            var gameId = gdr.GameId;
            var developerId = gdr.DeveloperId;
            var roleId = gdr.RoleId;

            await _unitOfWork.GameDeveloperRoles.DeleteAsync(id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var @event = new GameDeveloperRoleDeletedEvent
            {
                GameDeveloperRoleId = id,
                GameId = gameId,
                DeveloperId = developerId,
                RoleId = roleId
            };

            await _publishEndpoint.Publish(@event, cancellationToken);
            _logger.LogInformation(
                "Published GameDeveloperRoleDeletedEvent for GameId={GameId}, DeveloperId={DeveloperId}, RoleId={RoleId}",
                gameId, developerId, roleId
            );
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
