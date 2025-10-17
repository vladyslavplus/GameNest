namespace GameNest.Shared.Events.GamePlatforms
{
    public record GamePlatformCreatedEvent
    {
        public Guid GamePlatformId { get; init; }
        public Guid GameId { get; init; }
        public Guid PlatformId { get; init; }
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    }
}