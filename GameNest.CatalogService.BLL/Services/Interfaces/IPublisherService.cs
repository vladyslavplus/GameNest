using GameNest.CatalogService.BLL.DTOs.Publishers;
using GameNest.CatalogService.DAL.Helpers;
using GameNest.CatalogService.Domain.Entities.Parameters;

namespace GameNest.CatalogService.BLL.Services.Interfaces
{
    public interface IPublisherService
    {
        Task<PagedList<PublisherDto>> GetPublishersPagedAsync(PublisherParameters parameters, CancellationToken cancellationToken = default);
        Task<PublisherDto?> GetPublisherByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PublisherDto> CreatePublisherAsync(PublisherCreateDto createDto, CancellationToken cancellationToken = default);
        Task<PublisherDto> UpdatePublisherAsync(Guid id, PublisherUpdateDto updateDto, CancellationToken cancellationToken = default);
        Task DeletePublisherAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
