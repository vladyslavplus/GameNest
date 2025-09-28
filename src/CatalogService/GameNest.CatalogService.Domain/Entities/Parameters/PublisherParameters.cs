namespace GameNest.CatalogService.Domain.Entities.Parameters
{
    public class PublisherParameters : QueryStringParameters
    {
        public string? Name { get; set; }
        public string? Type { get; set; }
        public string? Country { get; set; }
        public string? Phone { get; set; }
    }
}
