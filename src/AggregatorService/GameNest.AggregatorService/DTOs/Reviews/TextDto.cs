namespace GameNest.AggregatorService.DTOs.Reviews
{
    public class TextDto
    {
        public string Value { get; set; } = default!;
        public int WordCount { get; set; }
        public bool IsLongReview { get; set; }
    }
}
