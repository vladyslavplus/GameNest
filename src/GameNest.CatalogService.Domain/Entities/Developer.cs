namespace GameNest.CatalogService.Domain.Entities
{
    public class Developer : BaseEntity
    {
        public string FullName { get; set; } = null!;
        public string? Email { get; set; }
        public string? Country { get; set; }
        public ICollection<GameDeveloperRole> GameDeveloperRoles { get; set; } = new List<GameDeveloperRole>();
    }
}