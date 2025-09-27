namespace GameNest.CatalogService.BLL.DTOs.Games
{
    public class GameCreateDto
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public decimal Price { get; set; }
        public Guid? PublisherId { get; set; }
    }
}