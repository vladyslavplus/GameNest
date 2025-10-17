namespace GameNest.Shared.Events.Developers
{
    public record DeveloperDeletedEvent
    {
        public Guid DeveloperId { get; init; }
        public string FullName { get; init; } = null!;
        public DateTime DeletedAt { get; init; } = DateTime.UtcNow;
    }
}