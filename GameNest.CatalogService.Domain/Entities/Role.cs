namespace GameNest.CatalogService.Domain.Entities
{
    public class Role : BaseEntity
    {
        public string Name { get; set; } = null!;
        public ICollection<GameDeveloperRole> GameDeveloperRoles { get; set; } = new List<GameDeveloperRole>();
    }
}