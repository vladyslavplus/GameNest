namespace GameNest.CatalogService.Domain.Entities.Parameters
{
    public class GameParameters : QueryStringParameters
    {
        public string? Title { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public Guid? PublisherId { get; set; }
    }
}