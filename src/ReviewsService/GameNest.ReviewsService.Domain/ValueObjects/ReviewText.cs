using GameNest.ReviewsService.Domain.Common;
using GameNest.ReviewsService.Domain.Exceptions;
using MongoDB.Bson.Serialization.Attributes;

namespace GameNest.ReviewsService.Domain.ValueObjects
{
    public sealed class ReviewText : ValueObject
    {
        [BsonElement("value")]
        public string Value { get; } = default!;

        [BsonIgnore]
        public int WordCount => Value.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;

        [BsonIgnore]
        public bool IsLongReview => WordCount > 50;

        [BsonConstructor]
        public ReviewText(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Review text cannot be empty");

            if (value.Length > 2000)
                throw new DomainException("Review text cannot exceed 2000 characters");

            if (value.Length < 10)
                throw new DomainException("Review text must be at least 10 characters");

            Value = value.Trim();
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value.ToLowerInvariant();
        }

        public override string ToString() => Value;

        public static ReviewText Create(string text) => new ReviewText(text);
    }
}