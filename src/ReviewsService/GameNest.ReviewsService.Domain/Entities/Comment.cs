using GameNest.ReviewsService.Domain.Common;
using GameNest.ReviewsService.Domain.Exceptions;
using GameNest.ReviewsService.Domain.ValueObjects;

namespace GameNest.ReviewsService.Domain.Entities
{
    public class Comment : BaseEntity
    {
        public string ReviewId { get; private set; } = default!;
        public string CustomerId { get; private set; } = default!;
        public ReviewText Text { get; private set; } = default!;
        public List<Reply> Replies { get; private set; } = new();

        private Comment() : base(null!) { }

        public Comment(string reviewId, string customerId, ReviewText text)
            : base(null)
        {
            if (string.IsNullOrWhiteSpace(reviewId))
                throw new DomainException("ReviewId is required");
            if (string.IsNullOrWhiteSpace(customerId))
                throw new DomainException("CustomerId is required");

            ReviewId = reviewId;
            CustomerId = customerId;
            Text = text ?? throw new DomainException("Text is required");
        }

        public Comment(string reviewId, string customerId, ReviewText text, string createdBy)
            : base(createdBy)
        {
            if (string.IsNullOrWhiteSpace(reviewId))
                throw new DomainException("ReviewId is required");
            if (string.IsNullOrWhiteSpace(customerId))
                throw new DomainException("CustomerId is required");

            ReviewId = reviewId;
            CustomerId = customerId;
            Text = text ?? throw new DomainException("Text is required");
        }

        public void AddReply(Reply reply) => Replies.Add(reply);
        public void UpdateText(ReviewText newText, string updatedBy)
        {
            Text = newText ?? throw new DomainException("Text is required");
            SetUpdated(updatedBy ?? "system");
        }
    }
}
