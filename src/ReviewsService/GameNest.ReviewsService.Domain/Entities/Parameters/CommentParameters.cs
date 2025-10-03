namespace GameNest.ReviewsService.Domain.Entities.Parameters
{
    public class CommentParameters : QueryStringParameters
    {
        public string? ReviewId { get; set; }
        public string? CustomerId { get; set; }
        public string? SearchText { get; set; }
        public string? CursorId { get; set; }
    }
}