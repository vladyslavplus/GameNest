namespace GameNest.CatalogService.BLL.DTOs.GamePlatforms
{
    public class GamePlatformDto
    {
        public Guid Id { get; set; }
        public Guid GameId { get; set; }
        public string GameTitle { get; set; } = null!;
        public Guid PlatformId { get; set; }
        public string PlatformName { get; set; } = null!;
    }
}
