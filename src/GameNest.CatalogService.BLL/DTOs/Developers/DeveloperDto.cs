namespace GameNest.CatalogService.BLL.DTOs.Developers
{
    public class DeveloperDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!;
        public string? Email { get; set; }
        public string? Country { get; set; }
        public List<DeveloperGameRoleDto> GameRoles { get; set; } = new();
    }
}
