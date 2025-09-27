namespace GameNest.CatalogService.Domain.Entities.Parameters
{
    public class GameGenreParameters : QueryStringParameters
    {
        public Guid? GameId { get; set; }
        public Guid? GenreId { get; set; }
    }
}
