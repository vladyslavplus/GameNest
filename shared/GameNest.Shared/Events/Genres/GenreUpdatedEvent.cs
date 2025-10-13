namespace GameNest.Shared.Events.Genres
{
    public record GenreUpdatedEvent
    {
        public Guid GenreId { get; init; }
        public string NewName { get; init; } = null!;
        public string? OldName { get; init; }
        public DateTime UpdatedAt { get; init; } = DateTime.UtcNow;
    }
}