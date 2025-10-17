namespace GameNest.Shared.Events.GameDeveloperRoles
{
    public record GameDeveloperRoleDeletedEvent
    {
        public Guid GameDeveloperRoleId { get; init; }
        public Guid GameId { get; init; }
        public Guid DeveloperId { get; init; }
        public Guid RoleId { get; init; }
        public DateTime DeletedAt { get; init; } = DateTime.UtcNow;
    }
}