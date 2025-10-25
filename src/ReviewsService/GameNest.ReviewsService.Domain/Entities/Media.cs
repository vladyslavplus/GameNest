using GameNest.ReviewsService.Domain.Common;
using GameNest.ReviewsService.Domain.Exceptions;
using GameNest.ReviewsService.Domain.ValueObjects;

namespace GameNest.ReviewsService.Domain.Entities
{
    public class Media : BaseEntity
    {
        public string GameId { get; private set; } = default!;
        public string CustomerId { get; private set; } = default!;
        public MediaUrl Url { get; private set; } = default!;

        private Media() : base(null!) { }

        public Media(string gameId, string customerId, MediaUrl url)
            : base(null)
        {
            if (string.IsNullOrWhiteSpace(gameId))
                throw new DomainException("GameId is required");
            if (string.IsNullOrWhiteSpace(customerId))
                throw new DomainException("CustomerId is required");

            GameId = gameId;
            CustomerId = customerId;
            Url = url ?? throw new DomainException("Url is required");
        }

        public Media(string gameId, string customerId, MediaUrl url, string createdBy)
            : base(createdBy)
        {
            if (string.IsNullOrWhiteSpace(gameId))
                throw new DomainException("GameId is required");
            if (string.IsNullOrWhiteSpace(customerId))
                throw new DomainException("CustomerId is required");

            GameId = gameId;
            CustomerId = customerId;
            Url = url ?? throw new DomainException("Url is required");
        }

        public void UpdateUrl(MediaUrl newUrl, string updatedBy)
        {
            Url = newUrl ?? throw new DomainException("Url is required");
            SetUpdated(updatedBy ?? "system");
        }
    }
}
