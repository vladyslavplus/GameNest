namespace GameNest.ReviewsService.Domain.Exceptions
{
    public class MongoDbConnectionException : Exception
    {
        public MongoDbConnectionException(string message)
            : base(message) { }

        public MongoDbConnectionException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}