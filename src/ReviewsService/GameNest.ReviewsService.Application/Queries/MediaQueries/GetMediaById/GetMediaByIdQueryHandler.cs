using GameNest.ReviewsService.Application.Interfaces.Queries;
using GameNest.ReviewsService.Domain.Entities;
using GameNest.ReviewsService.Domain.Interfaces.Services;

namespace GameNest.ReviewsService.Application.Queries.MediaQueries.GetMediaById
{
    public class GetMediaByIdQueryHandler : IQueryHandler<GetMediaByIdQuery, Media?>
    {
        private readonly IMediaService _mediaService;

        public GetMediaByIdQueryHandler(IMediaService mediaService)
        {
            _mediaService = mediaService;
        }

        public async Task<Media?> Handle(GetMediaByIdQuery request, CancellationToken cancellationToken)
        {
            return await _mediaService.GetMediaByIdAsync(request.MediaId, cancellationToken);
        }
    }
}