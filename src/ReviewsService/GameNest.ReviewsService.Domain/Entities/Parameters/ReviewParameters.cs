namespace GameNest.ReviewsService.Domain.Entities.Parameters
{
    public class ReviewParameters : QueryStringParameters
    {
        public string? GameId { get; set; }
        public string? CustomerId { get; set; }
        public string? SearchText { get; set; }
    }
}