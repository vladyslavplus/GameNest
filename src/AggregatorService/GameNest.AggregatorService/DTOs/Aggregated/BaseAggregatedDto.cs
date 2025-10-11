namespace GameNest.AggregatorService.DTOs.Aggregated
{
    public abstract class BaseAggregatedDto
    {
        public bool PartialData { get; set; }
        public string[] Warnings { get; set; } = Array.Empty<string>();
        public DateTime ResponseTimestamp { get; set; }
        public string Summary { get; set; } = string.Empty;
    }
}
