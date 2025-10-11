namespace GameNest.AggregatorService.DTOs.Catalog
{
    public class GameDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public decimal Price { get; set; }
        public PublisherDto? Publisher { get; set; }
        public List<string> Genres { get; set; } = new();
        public List<string> Platforms { get; set; } = new();
    }
}
