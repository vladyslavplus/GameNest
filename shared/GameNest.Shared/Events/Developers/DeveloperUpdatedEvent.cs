namespace GameNest.Shared.Events.Developers
{
    public record DeveloperUpdatedEvent
    {
        public Guid DeveloperId { get; init; }
        public string OldFullName { get; init; } = null!;
        public string? OldEmail { get; init; }
        public string? OldCountry { get; init; }
        public string NewFullName { get; init; } = null!;
        public string? NewEmail { get; init; }
        public string? NewCountry { get; init; }
        public DateTime UpdatedAt { get; init; } = DateTime.UtcNow;
    }
}