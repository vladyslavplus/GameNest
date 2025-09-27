namespace GameNest.CatalogService.Domain.Entities
{
    public class Genre : BaseEntity
    {
        public string Name { get; set; } = null!;
        public ICollection<GameGenre> GameGenres { get; set; } = new List<GameGenre>();
    }
}