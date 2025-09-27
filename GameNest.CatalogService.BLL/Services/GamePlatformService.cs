using AutoMapper;
using GameNest.CatalogService.BLL.DTOs.GamePlatforms;
using GameNest.CatalogService.BLL.Services.Interfaces;
using GameNest.CatalogService.DAL.Helpers;
using GameNest.CatalogService.DAL.UOW;
using GameNest.CatalogService.Domain.Entities;
using GameNest.CatalogService.Domain.Entities.Parameters;

namespace GameNest.CatalogService.BLL.Services
{
    public class GamePlatformService : IGamePlatformService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GamePlatformService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedList<GamePlatformDto>> GetGamePlatformsPagedAsync(GamePlatformParameters parameters, CancellationToken cancellationToken = default)
        {
            var gamePlatformsPaged = await _unitOfWork.GamePlatforms.GetGamePlatformsPagedAsync(parameters, cancellationToken: cancellationToken);
            var dtoList = gamePlatformsPaged.Select(gp => _mapper.Map<GamePlatformDto>(gp)).ToList();

            return new PagedList<GamePlatformDto>(
                dtoList,
                gamePlatformsPaged.TotalCount,
                gamePlatformsPaged.CurrentPage,
                gamePlatformsPaged.PageSize
            );
        }

        public async Task<GamePlatformDto?> GetGamePlatformByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var gamePlatform = await _unitOfWork.GamePlatforms.GetByIdWithReferencesAsync(id, cancellationToken);
            return gamePlatform == null ? null : _mapper.Map<GamePlatformDto>(gamePlatform);
        }

        public async Task<GamePlatformDto> CreateGamePlatformAsync(GamePlatformCreateDto createDto, CancellationToken cancellationToken = default)
        {
            var gamePlatform = _mapper.Map<GamePlatform>(createDto);

            await _unitOfWork.GamePlatforms.AddAsync(gamePlatform, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<GamePlatformDto>(gamePlatform);
        }

        public async Task<GamePlatformDto> UpdateGamePlatformAsync(Guid id, GamePlatformUpdateDto updateDto, CancellationToken cancellationToken = default)
        {
            var gamePlatform = await GetGamePlatformOrThrowAsync(id, cancellationToken);

            gamePlatform.GameId = updateDto.GameId ?? gamePlatform.GameId;
            gamePlatform.PlatformId = updateDto.PlatformId ?? gamePlatform.PlatformId;

            await _unitOfWork.GamePlatforms.UpdateAsync(gamePlatform);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<GamePlatformDto>(gamePlatform);
        }

        public async Task DeleteGamePlatformAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await GetGamePlatformOrThrowAsync(id, cancellationToken);
            await _unitOfWork.GamePlatforms.DeleteAsync(id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private async Task<GamePlatform> GetGamePlatformOrThrowAsync(Guid id, CancellationToken cancellationToken)
        {
            var gamePlatform = await _unitOfWork.GamePlatforms.GetByIdWithReferencesAsync(id, cancellationToken);
            if (gamePlatform == null)
                throw new KeyNotFoundException($"GamePlatform with id {id} not found.");
            return gamePlatform;
        }
    }
}
