using GameNest.ReviewsService.Domain.Exceptions;
using MongoDB.Bson.Serialization.Attributes;

namespace GameNest.ReviewsService.Domain.Common
{
    public abstract class BaseEntity
    {
        [BsonId]
        public string Id { get; private set; } = Guid.NewGuid().ToString();
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; private set; }
        public string? CreatedBy { get; private set; }
        public string? UpdatedBy { get; private set; }

        protected BaseEntity(string? createdBy = null)
        {
            CreatedBy = createdBy;
        }

        public void SetCreatedBy(string createdBy)
        {
            if (!string.IsNullOrWhiteSpace(CreatedBy))
                throw new DomainException("CreatedBy is already set");

            CreatedBy = createdBy ?? throw new DomainException("CreatedBy is required");
        }

        public void SetUpdated(string updatedBy)
        {
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = updatedBy ?? throw new DomainException("UpdatedBy is required");
        }
    }
}
