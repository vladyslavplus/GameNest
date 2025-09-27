namespace GameNest.CatalogService.BLL.DTOs.GameGenres
{
    public class GameGenreDto
    {
        public Guid Id { get; set; }

        public Guid GameId { get; set; }
        public string GameTitle { get; set; } = null!;

        public Guid GenreId { get; set; }
        public string GenreName { get; set; } = null!;
    }
}
