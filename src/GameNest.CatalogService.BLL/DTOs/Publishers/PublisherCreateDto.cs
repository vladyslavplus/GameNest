namespace GameNest.CatalogService.BLL.DTOs.Publishers
{
    public class PublisherCreateDto
    {
        public string Name { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string? Country { get; set; }
        public string? Phone { get; set; }
    }
}
