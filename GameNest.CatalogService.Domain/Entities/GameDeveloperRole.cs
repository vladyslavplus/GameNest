namespace GameNest.CatalogService.Domain.Entities
{
    public class GameDeveloperRole : BaseEntity
    {
        public Guid GameId { get; set; }
        public Game Game { get; set; } = null!;
        public Guid DeveloperId { get; set; }
        public Developer Developer { get; set; } = null!;
        public Guid RoleId { get; set; }
        public Role Role { get; set; } = null!;
        public string Seniority { get; set; } = null!;
    }
}