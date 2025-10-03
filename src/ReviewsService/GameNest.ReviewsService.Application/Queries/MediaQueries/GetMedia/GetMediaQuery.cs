using GameNest.ReviewsService.Application.Interfaces.Queries;
using GameNest.ReviewsService.Domain.Common.Helpers;
using GameNest.ReviewsService.Domain.Entities;
using GameNest.ReviewsService.Domain.Entities.Parameters;

namespace GameNest.ReviewsService.Application.Queries.MediaQueries.GetMedia
{
    public class GetMediaQuery : IQuery<PagedList<Media>>
    {
        public MediaParameters Parameters { get; init; } = default!;
    }
}