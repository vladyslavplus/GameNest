namespace GameNest.Shared.Events.Platforms
{
    public record PlatformUpdatedEvent
    {
        public Guid PlatformId { get; init; }
        public string NewName { get; init; } = null!;
        public string? OldName { get; init; }
        public DateTime UpdatedAt { get; init; } = DateTime.UtcNow;
    }
}