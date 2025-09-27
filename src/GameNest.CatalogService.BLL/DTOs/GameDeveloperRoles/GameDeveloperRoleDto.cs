namespace GameNest.CatalogService.BLL.DTOs.GameDeveloperRoles
{
    public class GameDeveloperRoleDto
    {
        public Guid Id { get; set; }

        public Guid GameId { get; set; }
        public string GameTitle { get; set; } = null!;

        public Guid DeveloperId { get; set; }
        public string DeveloperFullName { get; set; } = null!;

        public Guid RoleId { get; set; }
        public string RoleName { get; set; } = null!;

        public string Seniority { get; set; } = null!;
    }
}
