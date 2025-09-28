namespace GameNest.CatalogService.BLL.DTOs.Games
{
    public class GameDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public decimal Price { get; set; }

        public Guid PublisherId { get; set; }
        public string PublisherName { get; set; } = null!;

        public List<string> Genres { get; set; } = new();
        public List<string> Platforms { get; set; } = new();
        public List<string> Developers { get; set; } = new();
    }
}