namespace GameNest.AggregatorService.DTOs.Reviews
{
    public class ReplyDto
    {
        public string Id { get; set; } = default!;
        public string CustomerId { get; set; } = default!;
        public string Text { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
