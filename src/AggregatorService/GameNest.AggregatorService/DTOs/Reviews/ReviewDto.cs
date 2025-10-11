namespace GameNest.AggregatorService.DTOs.Reviews
{
    public class ReviewDto
    {
        public string Id { get; set; } = default!;
        public string GameId { get; set; } = default!;
        public string CustomerId { get; set; } = default!;
        public RatingDto Rating { get; set; } = default!;
        public TextDto Text { get; set; } = default!;
        public List<ReplyDto> Replies { get; set; } = new();
    }
}
