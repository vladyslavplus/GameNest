namespace GameNest.CatalogService.Domain.Entities
{
    public class Game : BaseEntity
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public decimal Price { get; set; }
        public Guid? PublisherId { get; set; }
        public Publisher? Publisher { get; set; } = null!;
        public ICollection<GameDeveloperRole> GameDeveloperRoles { get; set; } = new List<GameDeveloperRole>();
        public ICollection<GamePlatform> GamePlatforms { get; set; } = new List<GamePlatform>();
        public ICollection<GameGenre> GameGenres { get; set; } = new List<GameGenre>();
    }
}