namespace GameNest.ReviewsService.Infrastructure.Mongo.Seeding
{
    public interface IDataSeeder
    {
        Task SeedAsync(CancellationToken cancellationToken = default);
    }
}
