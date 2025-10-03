using GameNest.ReviewsService.Domain.Common;
using GameNest.ReviewsService.Domain.Exceptions;
using MongoDB.Bson.Serialization.Attributes;

namespace GameNest.ReviewsService.Domain.ValueObjects
{
    public sealed class Rating : ValueObject
    {
        [BsonElement("value")]
        public double Value { get; }

        private Rating() { }

        public Rating(double value)
        {
            if (value < 0 || value > 5)
                throw new DomainException("Rating must be between 0 and 5");

            Value = Math.Round(value, 1);
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => $"{Value:F1} ⭐";
        public static Rating Change(double newValue) => new Rating(newValue);
    }
}
