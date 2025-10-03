using GameNest.ReviewsService.Domain.Common;
using GameNest.ReviewsService.Domain.Exceptions;
using MongoDB.Bson.Serialization.Attributes;

namespace GameNest.ReviewsService.Domain.ValueObjects
{
    public sealed class MediaUrl : ValueObject
    {
        [BsonElement("value")]
        public string Value { get; } = default!;

        [BsonConstructor]
        public MediaUrl(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || !Uri.IsWellFormedUriString(value, UriKind.Absolute))
                throw new DomainException("Invalid media URL");

            Value = value;
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value.ToLowerInvariant();
        }

        public override string ToString() => Value;
    }
}