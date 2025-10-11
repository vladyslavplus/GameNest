using GameNest.AggregatorService.DTOs.Catalog;
using GameNest.AggregatorService.DTOs.Reviews;

namespace GameNest.AggregatorService.DTOs.Aggregated
{
    public class AggregatedGameDto : BaseAggregatedDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public decimal Price { get; set; }
        public PublisherDto? Publisher { get; set; }
        public List<string> Genres { get; set; } = new();
        public List<string> Platforms { get; set; } = new();
        public List<ReviewDto> Reviews { get; set; } = new();
        public int ReviewCount { get; set; }
        public double? AverageRating { get; set; }
    }
}
