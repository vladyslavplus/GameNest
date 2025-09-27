namespace GameNest.CatalogService.BLL.DTOs.GameDeveloperRoles
{
    public class GameDeveloperRoleCreateDto
    {
        public Guid GameId { get; set; }
        public Guid DeveloperId { get; set; }
        public Guid RoleId { get; set; }
        public string Seniority { get; set; } = null!;
    }
}
