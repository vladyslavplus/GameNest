namespace GameNest.CatalogService.BLL.DTOs.Developers
{
    public class DeveloperGameRoleDto
    {
        public Guid GameId { get; set; }
        public string GameName { get; set; } = null!;
        public string RoleName { get; set; } = null!;
        public string Seniority { get; set; } = null!;
    }
}
