namespace GameNest.Shared.Events.Roles
{
    public record RoleUpdatedEvent
    {
        public Guid RoleId { get; init; }
        public string? OldName { get; init; }
        public string NewName { get; init; } = null!;
        public DateTime UpdatedAt { get; init; } = DateTime.UtcNow;
    }
}