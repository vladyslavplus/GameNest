namespace GameNest.Shared.Events.Publishers
{
    public record PublisherDeletedEvent
    {
        public Guid PublisherId { get; init; }
        public string PublisherName { get; init; } = null!;
        public DateTime DeletedAt { get; init; } = DateTime.UtcNow;
    }
}