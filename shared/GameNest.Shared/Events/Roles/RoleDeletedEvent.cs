namespace GameNest.Shared.Events.Roles
{
    public record RoleDeletedEvent
    {
        public Guid RoleId { get; init; }
        public string RoleName { get; init; } = null!;
        public DateTime DeletedAt { get; init; } = DateTime.UtcNow;
    }
}