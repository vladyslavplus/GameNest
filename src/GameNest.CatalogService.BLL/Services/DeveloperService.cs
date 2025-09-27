using AutoMapper;
using GameNest.CatalogService.BLL.DTOs.Developers;
using GameNest.CatalogService.BLL.Services.Interfaces;
using GameNest.CatalogService.DAL.Helpers;
using GameNest.CatalogService.DAL.UOW;
using GameNest.CatalogService.Domain.Entities;
using GameNest.CatalogService.Domain.Entities.Parameters;

namespace GameNest.CatalogService.BLL.Services
{
    public class DeveloperService : IDeveloperService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DeveloperService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedList<DeveloperDto>> GetDevelopersPagedAsync(DeveloperParameters parameters, CancellationToken cancellationToken = default)
        {
            var developersPaged = await _unitOfWork.Developers.GetDevelopersPagedAsync(parameters, cancellationToken: cancellationToken);
            var dtoList = developersPaged.Select(d => _mapper.Map<DeveloperDto>(d)).ToList();

            return new PagedList<DeveloperDto>(
                dtoList,
                developersPaged.TotalCount,
                developersPaged.CurrentPage,
                developersPaged.PageSize
            );
        }

        public async Task<DeveloperDto?> GetDeveloperByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var developer = await _unitOfWork.Developers.GetByIdWithDetailsAsync(id, cancellationToken: cancellationToken);
            return developer == null ? null : _mapper.Map<DeveloperDto>(developer);
        }

        public async Task<DeveloperDto> CreateDeveloperAsync(DeveloperCreateDto developerCreateDto, CancellationToken cancellationToken = default)
        {
            var developer = _mapper.Map<Developer>(developerCreateDto);
            await _unitOfWork.Developers.AddAsync(developer, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<DeveloperDto>(developer);
        }

        public async Task<DeveloperDto> UpdateDeveloperAsync(Guid id, DeveloperUpdateDto updateDto, CancellationToken cancellationToken = default)
        {
            var developer = await GetDeveloperOrThrowAsync(id, cancellationToken);

            developer.FullName = updateDto.FullName ?? developer.FullName;
            developer.Email = updateDto.Email ?? developer.Email;
            developer.Country = updateDto.Country ?? developer.Country;

            await _unitOfWork.Developers.UpdateAsync(developer);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<DeveloperDto>(developer);
        }

        public async Task DeleteDeveloperAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var developer = await GetDeveloperOrThrowAsync(id, cancellationToken);

            await _unitOfWork.Developers.DeleteAsync(developer.Id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private async Task<Developer> GetDeveloperOrThrowAsync(Guid id, CancellationToken cancellationToken)
        {
            var developer = await _unitOfWork.Developers.GetByIdWithDetailsAsync(id, cancellationToken);
            if (developer == null)
                throw new KeyNotFoundException($"Developer with id {id} not found.");
            return developer;
        }
    }
}
