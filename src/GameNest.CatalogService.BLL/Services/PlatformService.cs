using AutoMapper;
using GameNest.CatalogService.BLL.DTOs.Platforms;
using GameNest.CatalogService.BLL.Services.Interfaces;
using GameNest.CatalogService.DAL.Helpers;
using GameNest.CatalogService.DAL.UOW;
using GameNest.CatalogService.Domain.Entities;
using GameNest.CatalogService.Domain.Entities.Parameters;

namespace GameNest.CatalogService.BLL.Services
{
    public class PlatformService : IPlatformService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PlatformService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedList<PlatformDto>> GetPlatformsPagedAsync(PlatformParameters parameters, CancellationToken cancellationToken = default)
        {
            var platformsPaged = await _unitOfWork.Platforms.GetPlatformsPagedAsync(parameters, cancellationToken: cancellationToken);
            var dtoList = platformsPaged.Select(p => _mapper.Map<PlatformDto>(p)).ToList();

            return new PagedList<PlatformDto>(
                dtoList,
                platformsPaged.TotalCount,
                platformsPaged.CurrentPage,
                platformsPaged.PageSize
            );
        }

        public async Task<PlatformDto?> GetPlatformByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var platform = await _unitOfWork.Platforms.GetByIdAsync(id, cancellationToken: cancellationToken);
            return platform == null ? null : _mapper.Map<PlatformDto>(platform);
        }

        public async Task<PlatformDto> CreatePlatformAsync(PlatformCreateDto dto, CancellationToken cancellationToken = default)
        {
            var platform = _mapper.Map<Platform>(dto);
            await _unitOfWork.Platforms.AddAsync(platform, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<PlatformDto>(platform);
        }

        public async Task<PlatformDto> UpdatePlatformAsync(Guid id, PlatformUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var platform = await GetPlatformOrThrowAsync(id, cancellationToken);

            platform.Name = dto.Name ?? platform.Name;

            await _unitOfWork.Platforms.UpdateAsync(platform);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<PlatformDto>(platform);
        }

        public async Task DeletePlatformAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await GetPlatformOrThrowAsync(id, cancellationToken);
            await _unitOfWork.Platforms.DeleteAsync(id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private async Task<Platform> GetPlatformOrThrowAsync(Guid id, CancellationToken cancellationToken)
        {
            var platform = await _unitOfWork.Platforms.GetByIdAsync(id, cancellationToken: cancellationToken);
            if (platform == null)
                throw new KeyNotFoundException($"Platform with id {id} not found.");
            return platform;
        }
    }
}
