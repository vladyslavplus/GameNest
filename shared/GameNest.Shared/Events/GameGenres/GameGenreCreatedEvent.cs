namespace GameNest.Shared.Events.GameGenres
{
    public record GameGenreCreatedEvent
    {
        public Guid GameGenreId { get; init; }
        public Guid GameId { get; init; }
        public Guid GenreId { get; init; }
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    }
}