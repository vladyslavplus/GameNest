namespace GameNest.Shared.Events.Genres
{
    public record GenreDeletedEvent
    {
        public Guid GenreId { get; init; }
        public string GenreName { get; init; } = null!;
        public DateTime DeletedAt { get; init; } = DateTime.UtcNow;
    }
}