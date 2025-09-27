namespace GameNest.CatalogService.Domain.Entities.Parameters
{
    public class DeveloperParameters : QueryStringParameters
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Country { get; set; }
    }
}
