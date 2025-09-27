namespace GameNest.CatalogService.Domain.Entities
{
    public class Platform : BaseEntity
    {
        public string Name { get; set; } = null!;
        public ICollection<GamePlatform> GamePlatforms { get; set; } = new List<GamePlatform>();
    }
}