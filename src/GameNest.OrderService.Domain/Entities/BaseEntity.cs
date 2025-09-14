namespace GameNest.OrderService.Domain.Entities
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();  
        public DateTime Created_At { get; set; } = DateTime.UtcNow;
        public Guid? Created_By { get; set; }
        public DateTime Updated_At { get; set; } = DateTime.UtcNow;
        public Guid? Updated_By { get; set; }
        public bool Is_Deleted { get; set; } = false;
    }
}
