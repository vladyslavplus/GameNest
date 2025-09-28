using AutoMapper;
using GameNest.CatalogService.BLL.DTOs.Publishers;
using GameNest.CatalogService.BLL.Services.Interfaces;
using GameNest.CatalogService.DAL.Helpers;
using GameNest.CatalogService.DAL.UOW;
using GameNest.CatalogService.Domain.Entities;
using GameNest.CatalogService.Domain.Entities.Parameters;

namespace GameNest.CatalogService.BLL.Services
{
    public class PublisherService : IPublisherService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PublisherService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedList<PublisherDto>> GetPublishersPagedAsync(PublisherParameters parameters, CancellationToken cancellationToken = default)
        {
            var publishersPaged = await _unitOfWork.Publishers.GetPublishersPagedAsync(parameters, cancellationToken: cancellationToken);
            var dtoList = publishersPaged.Select(p => _mapper.Map<PublisherDto>(p)).ToList();

            return new PagedList<PublisherDto>(
                dtoList,
                publishersPaged.TotalCount,
                publishersPaged.CurrentPage,
                publishersPaged.PageSize
            );
        }

        public async Task<PublisherDto?> GetPublisherByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var publisher = await _unitOfWork.Publishers.GetByIdWithGamesAsync(id, cancellationToken);
            return publisher == null ? null : _mapper.Map<PublisherDto>(publisher);
        }

        public async Task<PublisherDto> CreatePublisherAsync(PublisherCreateDto createDto, CancellationToken cancellationToken = default)
        {
            var publisher = _mapper.Map<Publisher>(createDto);
            await _unitOfWork.Publishers.AddAsync(publisher, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<PublisherDto>(publisher);
        }

        public async Task<PublisherDto> UpdatePublisherAsync(Guid id, PublisherUpdateDto updateDto, CancellationToken cancellationToken = default)
        {
            var publisher = await GetPublisherOrThrowAsync(id, cancellationToken);

            publisher.Name = updateDto.Name ?? publisher.Name;
            publisher.Type = updateDto.Type ?? publisher.Type;
            publisher.Country = updateDto.Country ?? publisher.Country;
            publisher.Phone = updateDto.Phone ?? publisher.Phone;

            await _unitOfWork.Publishers.UpdateAsync(publisher);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<PublisherDto>(publisher);
        }

        public async Task DeletePublisherAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var publisher = await GetPublisherOrThrowAsync(id, cancellationToken);

            await _unitOfWork.Publishers.DeleteAsync(publisher.Id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private async Task<Publisher> GetPublisherOrThrowAsync(Guid id, CancellationToken cancellationToken)
        {
            var publisher = await _unitOfWork.Publishers.GetByIdWithGamesAsync(id, cancellationToken);
            if (publisher == null)
                throw new KeyNotFoundException($"Publisher with id {id} not found.");
            return publisher;
        }
    }
}
