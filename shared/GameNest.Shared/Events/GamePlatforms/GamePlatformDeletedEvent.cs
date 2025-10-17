namespace GameNest.Shared.Events.GamePlatforms
{
    public record GamePlatformDeletedEvent
    {
        public Guid GamePlatformId { get; init; }
        public Guid GameId { get; init; }
        public Guid PlatformId { get; init; }
        public DateTime DeletedAt { get; init; } = DateTime.UtcNow;
    }
}