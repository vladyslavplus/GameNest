namespace GameNest.Shared.Events.GameGenres
{
    public record GameGenreDeletedEvent
    {
        public Guid GameGenreId { get; init; }
        public Guid GameId { get; init; }
        public Guid GenreId { get; init; }
        public DateTime DeletedAt { get; init; } = DateTime.UtcNow;
    }
}