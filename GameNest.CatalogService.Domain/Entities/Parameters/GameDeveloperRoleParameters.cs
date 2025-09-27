namespace GameNest.CatalogService.Domain.Entities.Parameters
{
    public class GameDeveloperRoleParameters : QueryStringParameters
    {
        public Guid? GameId { get; set; }
        public Guid? DeveloperId { get; set; }
        public Guid? RoleId { get; set; }
        public string? Seniority { get; set; }
    }
}
