namespace GameNest.ReviewsService.Domain.Entities.Parameters
{
    public class MediaParameters : QueryStringParameters
    {
        public string? GameId { get; set; }
        public string? CustomerId { get; set; }
        public string? UrlContains { get; set; }
    }
}