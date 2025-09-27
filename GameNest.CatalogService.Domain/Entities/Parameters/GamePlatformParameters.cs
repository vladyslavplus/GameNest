namespace GameNest.CatalogService.Domain.Entities.Parameters
{
    public class GamePlatformParameters : QueryStringParameters
    {
        public Guid? GameId { get; set; }
        public Guid? PlatformId { get; set; }
    }
}
