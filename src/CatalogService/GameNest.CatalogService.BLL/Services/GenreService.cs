using AutoMapper;
using GameNest.CatalogService.BLL.DTOs.Genres;
using GameNest.CatalogService.BLL.Services.Interfaces;
using GameNest.CatalogService.DAL.Helpers;
using GameNest.CatalogService.DAL.UOW;
using GameNest.CatalogService.Domain.Entities;
using GameNest.CatalogService.Domain.Entities.Parameters;
using GameNest.Shared.Events.Genres;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GameNest.CatalogService.BLL.Services
{
    public class GenreService : IGenreService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<GenreService> _logger;

        public GenreService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IPublishEndpoint publishEndpoint,
            ILogger<GenreService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task<PagedList<GenreDto>> GetGenresPagedAsync(GenreParameters parameters, CancellationToken cancellationToken = default)
        {
            var genresPaged = await _unitOfWork.Genres.GetGenresPagedAsync(parameters, cancellationToken: cancellationToken);
            var dtoList = genresPaged.Select(g => _mapper.Map<GenreDto>(g)).ToList();

            return new PagedList<GenreDto>(
                dtoList,
                genresPaged.TotalCount,
                genresPaged.CurrentPage,
                genresPaged.PageSize
            );
        }

        public async Task<GenreDto?> GetGenreByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var genre = await _unitOfWork.Genres.GetByIdAsync(id, cancellationToken: cancellationToken);
            return genre == null ? null : _mapper.Map<GenreDto>(genre);
        }

        public async Task<GenreDto> CreateGenreAsync(GenreCreateDto genreCreateDto, CancellationToken cancellationToken = default)
        {
            var genre = _mapper.Map<Genre>(genreCreateDto);
            await _unitOfWork.Genres.AddAsync(genre, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<GenreDto>(genre);
        }

        public async Task<GenreDto> UpdateGenreAsync(Guid id, GenreUpdateDto genreUpdateDto, CancellationToken cancellationToken = default)
        {
            var genre = await GetGenreOrThrowAsync(id, cancellationToken);

            var oldName = genre.Name; 
            genre.Name = genreUpdateDto.Name ?? genre.Name;

            await _unitOfWork.Genres.UpdateAsync(genre);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var @event = new GenreUpdatedEvent
            {
                GenreId = genre.Id,
                NewName = genre.Name,
                OldName = oldName
            };

            await _publishEndpoint.Publish(@event, cancellationToken);
            _logger.LogInformation("Published GenreUpdatedEvent for Genre {GenreId}: {OldName} -> {NewName}", genre.Id, oldName, genre.Name);

            return _mapper.Map<GenreDto>(genre);
        }

        public async Task DeleteGenreAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var genre = await GetGenreOrThrowAsync(id, cancellationToken);
            var genreName = genre.Name; 

            await _unitOfWork.Genres.DeleteAsync(id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var @event = new GenreDeletedEvent
            {
                GenreId = id,
                GenreName = genreName
            };


            await _publishEndpoint.Publish(@event, cancellationToken);
            _logger.LogInformation("Published GenreDeletedEvent for Genre {GenreId}: {GenreName}", id, genreName);
        }

        private async Task<Genre> GetGenreOrThrowAsync(Guid id, CancellationToken cancellationToken)
        {
            var genre = await _unitOfWork.Genres.GetByIdAsync(id, cancellationToken: cancellationToken);
            if (genre == null)
                throw new KeyNotFoundException($"Genre with id {id} not found.");
            return genre;
        }
    }
}