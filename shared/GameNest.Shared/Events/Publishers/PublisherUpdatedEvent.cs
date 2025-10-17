namespace GameNest.Shared.Events.Publishers
{
    public record PublisherUpdatedEvent
    {
        public Guid PublisherId { get; init; }

        public string? OldName { get; init; }
        public string? OldType { get; init; }
        public string? OldCountry { get; init; }
        public string? OldPhone { get; init; }

        public string NewName { get; init; } = null!;
        public string? NewType { get; init; }
        public string? NewCountry { get; init; }
        public string? NewPhone { get; init; }

        public DateTime UpdatedAt { get; init; } = DateTime.UtcNow;
    }
}