using GameNest.ReviewsService.Domain.Exceptions;
using GameNest.ReviewsService.Domain.ValueObjects;

namespace GameNest.ReviewsService.Domain.Entities
{
    public class Reply
    {
        public string Id { get; private set; } = Guid.NewGuid().ToString();
        public string CustomerId { get; private set; } = default!;
        public ReviewText Text { get; private set; } = default!;
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        private Reply() { }

        public Reply(string customerId, ReviewText text)
        {
            if (string.IsNullOrWhiteSpace(customerId))
                throw new DomainException("CustomerId is required");

            CustomerId = customerId;
            Text = text ?? throw new DomainException("Text is required");
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdateText(ReviewText newText)
        {
            Text = newText ?? throw new DomainException("Text is required");
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
