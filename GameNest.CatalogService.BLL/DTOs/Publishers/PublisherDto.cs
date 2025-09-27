namespace GameNest.CatalogService.BLL.DTOs.Publishers
{
    public class PublisherDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string? Country { get; set; }
        public string? Phone { get; set; }
        public List<string> Games { get; set; } = new();
    }
}
