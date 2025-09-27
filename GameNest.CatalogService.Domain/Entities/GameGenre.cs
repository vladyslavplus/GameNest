namespace GameNest.CatalogService.Domain.Entities
{
    public class GameGenre : BaseEntity
    {
        public Guid GameId { get; set; }
        public Game Game { get; set; } = null!;
        public Guid GenreId { get; set; }
        public Genre Genre { get; set; } = null!;
    }
}