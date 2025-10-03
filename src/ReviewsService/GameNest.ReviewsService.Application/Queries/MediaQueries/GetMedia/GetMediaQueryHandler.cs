using GameNest.ReviewsService.Application.Interfaces.Queries;
using GameNest.ReviewsService.Domain.Common.Helpers;
using GameNest.ReviewsService.Domain.Entities;
using GameNest.ReviewsService.Domain.Interfaces.Services;

namespace GameNest.ReviewsService.Application.Queries.MediaQueries.GetMedia
{
    public class GetMediaQueryHandler : IQueryHandler<GetMediaQuery, PagedList<Media>>
    {
        private readonly IMediaService _mediaService;

        public GetMediaQueryHandler(IMediaService mediaService)
        {
            _mediaService = mediaService;
        }

        public async Task<PagedList<Media>> Handle(GetMediaQuery request, CancellationToken cancellationToken)
        {
            return await _mediaService.GetMediaAsync(request.Parameters, cancellationToken);
        }
    }
}