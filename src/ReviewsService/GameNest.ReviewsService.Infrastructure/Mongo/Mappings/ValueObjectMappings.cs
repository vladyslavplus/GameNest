using GameNest.ReviewsService.Domain.Entities;
using GameNest.ReviewsService.Domain.ValueObjects;
using MongoDB.Bson.Serialization;

namespace GameNest.ReviewsService.Infrastructure.Mongo.Mappings
{
    public static class ValueObjectMappings
    {
        public static void Register()
        {
            BsonClassMap.RegisterClassMap<ReviewText>(cm =>
            {
                cm.AutoMap();
                cm.MapCreator(v => new ReviewText(v.Value));
            });

            BsonClassMap.RegisterClassMap<Rating>(cm =>
            {
                cm.AutoMap();
                cm.MapCreator(v => new Rating(v.Value));
            });

            BsonClassMap.RegisterClassMap<MediaUrl>(cm =>
            {
                cm.AutoMap();
                cm.MapCreator(v => new MediaUrl(v.Value));
            });

            BsonClassMap.RegisterClassMap<Reply>(cm => cm.AutoMap());
            BsonClassMap.RegisterClassMap<Comment>(cm => cm.AutoMap());
            BsonClassMap.RegisterClassMap<Review>(cm => cm.AutoMap());
            BsonClassMap.RegisterClassMap<Media>(cm => cm.AutoMap());
        }
    }
}