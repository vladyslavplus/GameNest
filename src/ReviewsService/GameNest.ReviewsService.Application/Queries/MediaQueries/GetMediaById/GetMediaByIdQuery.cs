using GameNest.ReviewsService.Application.Interfaces.Queries;
using GameNest.ReviewsService.Domain.Entities;

namespace GameNest.ReviewsService.Application.Queries.MediaQueries.GetMediaById
{
    public class GetMediaByIdQuery : IQuery<Media?>
    {
        public string MediaId { get; init; } = default!;
    }
}