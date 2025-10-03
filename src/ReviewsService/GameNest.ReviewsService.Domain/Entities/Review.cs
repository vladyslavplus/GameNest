using GameNest.ReviewsService.Domain.Common;
using GameNest.ReviewsService.Domain.Exceptions;
using GameNest.ReviewsService.Domain.ValueObjects;

namespace GameNest.ReviewsService.Domain.Entities
{
    public class Review : BaseEntity
    {
        public string GameId { get; private set; } = default!;
        public string CustomerId { get; private set; } = default!;
        public Rating Rating { get; private set; } = default!;
        public ReviewText Text { get; private set; } = default!;
        public List<Reply> Replies { get; private set; } = new();

        private Review() : base(null!) { } 

        public Review(string gameId, string customerId, Rating rating, ReviewText text, string createdBy)
            : base(createdBy)
        {
            if (string.IsNullOrWhiteSpace(gameId))
                throw new DomainException("GameId is required");
            if (string.IsNullOrWhiteSpace(customerId))
                throw new DomainException("CustomerId is required");

            GameId = gameId;
            CustomerId = customerId;
            Rating = rating ?? throw new DomainException("Rating is required");
            Text = text ?? throw new DomainException("Text is required");
        }

        public void AddReply(Reply reply)
        {
            if (reply is null) throw new DomainException("Reply cannot be null");
            Replies.Add(reply);
        }
        public void UpdateRating(Rating rating)
        {
            Rating = rating ?? throw new DomainException("Rating is required");
            SetUpdated("system");
        }
        public void UpdateText(ReviewText text)
        {
            Text = text ?? throw new DomainException("Text is required");
            SetUpdated("system");
        }
    }
}
