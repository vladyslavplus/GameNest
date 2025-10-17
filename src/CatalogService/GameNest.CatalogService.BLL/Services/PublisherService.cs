using AutoMapper;
using GameNest.CatalogService.BLL.DTOs.Publishers;
using GameNest.CatalogService.BLL.Services.Interfaces;
using GameNest.CatalogService.DAL.Helpers;
using GameNest.CatalogService.DAL.UOW;
using GameNest.CatalogService.Domain.Entities;
using GameNest.CatalogService.Domain.Entities.Parameters;
using GameNest.Shared.Events.Publishers;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GameNest.CatalogService.BLL.Services
{
    public class PublisherService : IPublisherService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<PublisherService> _logger;

        public PublisherService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IPublishEndpoint publishEndpoint,
            ILogger<PublisherService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
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

            var oldName = publisher.Name;
            var oldType = publisher.Type;
            var oldCountry = publisher.Country;
            var oldPhone = publisher.Phone;

            publisher.Name = updateDto.Name ?? publisher.Name;
            publisher.Type = updateDto.Type ?? publisher.Type;
            publisher.Country = updateDto.Country ?? publisher.Country;
            publisher.Phone = updateDto.Phone ?? publisher.Phone;

            await _unitOfWork.Publishers.UpdateAsync(publisher);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var @event = new PublisherUpdatedEvent
            {
                PublisherId = publisher.Id,
                OldName = oldName,
                OldType = oldType,
                OldCountry = oldCountry,
                OldPhone = oldPhone,
                NewName = publisher.Name,
                NewType = publisher.Type,
                NewCountry = publisher.Country,
                NewPhone = publisher.Phone
            };

            await _publishEndpoint.Publish(@event, cancellationToken);
            _logger.LogInformation("Published PublisherUpdatedEvent for Publisher {PublisherId}: {OldName} -> {NewName}",
                publisher.Id, oldName, publisher.Name);

            return _mapper.Map<PublisherDto>(publisher);
        }

        public async Task DeletePublisherAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var publisher = await GetPublisherOrThrowAsync(id, cancellationToken);
            var publisherName = publisher.Name;

            await _unitOfWork.Publishers.DeleteAsync(publisher.Id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var @event = new PublisherDeletedEvent
            {
                PublisherId = id,
                PublisherName = publisherName
            };

            await _publishEndpoint.Publish(@event, cancellationToken);
            _logger.LogInformation("Published PublisherDeletedEvent for Publisher {PublisherId}: {PublisherName}",
                id, publisherName);
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
