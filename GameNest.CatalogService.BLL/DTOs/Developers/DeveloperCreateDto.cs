namespace GameNest.CatalogService.BLL.DTOs.Developers
{
    public class DeveloperCreateDto
    {
        public string FullName { get; set; } = null!;
        public string? Email { get; set; }
        public string? Country { get; set; }
    }
}
