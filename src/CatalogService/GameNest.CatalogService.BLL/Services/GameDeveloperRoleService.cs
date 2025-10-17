using AutoMapper;
using GameNest.CatalogService.BLL.Cache.Services.Interfaces;
using GameNest.CatalogService.BLL.DTOs.GameDeveloperRoles;
using GameNest.CatalogService.BLL.Services.Interfaces;
using GameNest.CatalogService.DAL.Helpers;
using GameNest.CatalogService.DAL.UOW;
using GameNest.CatalogService.Domain.Entities;
using GameNest.CatalogService.Domain.Entities.Parameters;
using GameNest.ServiceDefaults.Hybrid;
using GameNest.Shared.Events.GameDeveloperRoles;
using GameNest.Shared.Helpers;
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
        private readonly IEntityCacheInvalidationService<GameDeveloperRole> _cacheInvalidationService;
        private readonly IHybridCacheService _cacheService;
        private const string CachePrefix = "gamedevrole";

        public GameDeveloperRoleService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IPublishEndpoint publishEndpoint,
            ILogger<GameDeveloperRoleService> logger,
            IEntityCacheInvalidationService<GameDeveloperRole> cacheInvalidationService,
            IHybridCacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
            _cacheInvalidationService = cacheInvalidationService;
            _cacheService = cacheService;
        }

        public async Task<PagedList<GameDeveloperRoleDto>> GetRolesPagedAsync(GameDeveloperRoleParameters parameters, CancellationToken cancellationToken = default)
        {
            string cacheKey = GenerateRolesListCacheKey(parameters);

            var cachedDto = await _cacheService.GetOrSetAsync(
                cacheKey,
                async () =>
                {
                    var rolesPaged = await _unitOfWork.GameDeveloperRoles.GetRolesPagedAsync(parameters, cancellationToken: cancellationToken);
                    if (rolesPaged is null || !rolesPaged.Any())
                        return null;

                    var dtoList = rolesPaged.Select(r => _mapper.Map<GameDeveloperRoleDto>(r)).ToList();
                    return new PagedListCacheDto<GameDeveloperRoleDto>
                    {
                        Items = dtoList,
                        TotalCount = rolesPaged.TotalCount,
                        PageNumber = rolesPaged.CurrentPage,
                        PageSize = rolesPaged.PageSize
                    };
                },
                memoryExpiration: TimeSpan.FromSeconds(30),
                redisExpiration: TimeSpan.FromMinutes(5)
            );

            if (cachedDto is null)
                return new PagedList<GameDeveloperRoleDto>(new List<GameDeveloperRoleDto>(), 0, parameters.PageNumber, parameters.PageSize);

            return new PagedList<GameDeveloperRoleDto>(
                cachedDto.Items,
                cachedDto.TotalCount,
                cachedDto.PageNumber,
                cachedDto.PageSize
            );
        }

        public async Task<GameDeveloperRoleDto?> GetRoleByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            string cacheKey = $"{CachePrefix}:{id}";

            return await _cacheService.GetOrSetAsync(
                cacheKey,
                async () =>
                {
                    var gdr = await _unitOfWork.GameDeveloperRoles.GetByIdWithReferencesAsync(id, cancellationToken);
                    return gdr is null ? null : _mapper.Map<GameDeveloperRoleDto>(gdr);
                },
                memoryExpiration: TimeSpan.FromMinutes(2),
                redisExpiration: TimeSpan.FromMinutes(30)
            );
        }

        public async Task<GameDeveloperRoleDto> CreateRoleAsync(GameDeveloperRoleCreateDto createDto, CancellationToken cancellationToken = default)
        {
            var gdr = _mapper.Map<GameDeveloperRole>(createDto);

            await _unitOfWork.GameDeveloperRoles.AddAsync(gdr, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            gdr = await _unitOfWork.GameDeveloperRoles.GetByIdWithReferencesAsync(gdr.Id, cancellationToken)
                  ?? throw new InvalidOperationException("Failed to load GameDeveloperRole after creation.");

            var createdDto = _mapper.Map<GameDeveloperRoleDto>(gdr);

            await _cacheInvalidationService.InvalidateAllAsync();

            await _cacheService.SetAsync($"{CachePrefix}:{gdr.Id}", createdDto,
                memoryExpiration: TimeSpan.FromMinutes(2),
                redisExpiration: TimeSpan.FromMinutes(30));

            var @event = new GameDeveloperRoleCreatedEvent
            {
                GameDeveloperRoleId = gdr.Id,
                GameId = gdr.GameId,
                DeveloperId = gdr.DeveloperId,
                RoleId = gdr.RoleId
            };

            await _publishEndpoint.Publish(@event, cancellationToken);
            _logger.LogInformation("Published GameDeveloperRoleCreatedEvent for GameId={GameId}, DeveloperId={DeveloperId}, RoleId={RoleId}",
                gdr.GameId, gdr.DeveloperId, gdr.RoleId);

            return createdDto;
        }

        public async Task DeleteRoleAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var gdr = await GetRoleOrThrowAsync(id, cancellationToken);

            await _unitOfWork.GameDeveloperRoles.DeleteAsync(id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _cacheInvalidationService.InvalidateByIdAsync(id);

            var @event = new GameDeveloperRoleDeletedEvent
            {
                GameDeveloperRoleId = id,
                GameId = gdr.GameId,
                DeveloperId = gdr.DeveloperId,
                RoleId = gdr.RoleId
            };

            await _publishEndpoint.Publish(@event, cancellationToken);
            _logger.LogInformation("Published GameDeveloperRoleDeletedEvent for GameId={GameId}, DeveloperId={DeveloperId}, RoleId={RoleId}",
                gdr.GameId, gdr.DeveloperId, gdr.RoleId);
        }

        private async Task<GameDeveloperRole> GetRoleOrThrowAsync(Guid id, CancellationToken cancellationToken)
        {
            var gdr = await _unitOfWork.GameDeveloperRoles.GetByIdWithReferencesAsync(id, cancellationToken);
            if (gdr == null)
                throw new KeyNotFoundException($"GameDeveloperRole with id {id} not found.");
            return gdr;
        }

        public static string GenerateRolesListCacheKey(GameDeveloperRoleParameters parameters)
        {
            return $"gamedevroles:page:{parameters.PageNumber}"
                 + $":size:{parameters.PageSize}"
                 + $":order:{parameters.OrderBy ?? "Id"}"
                 + $":game:{parameters.GameId?.ToString() ?? ""}"
                 + $":developer:{parameters.DeveloperId?.ToString() ?? ""}"
                 + $":role:{parameters.RoleId?.ToString() ?? ""}"
                 + $":seniority:{parameters.Seniority ?? ""}";
        }
    }
}
