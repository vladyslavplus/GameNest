namespace GameNest.CatalogService.Domain.Entities
{
    public class Publisher : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string? Country { get; set; }
        public string? Phone { get; set; }
        public ICollection<Game> Games { get; set; } = new List<Game>();
    }
}