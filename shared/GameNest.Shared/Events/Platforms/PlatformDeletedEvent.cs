namespace GameNest.Shared.Events.Platforms
{
    public record PlatformDeletedEvent
    {
        public Guid PlatformId { get; init; }
        public string PlatformName { get; init; } = null!;
        public DateTime DeletedAt { get; init; } = DateTime.UtcNow;
    }
}