namespace GameNest.ReviewsService.Domain.Exceptions
{
    public class MongoDbWriteException : Exception
    {
        public MongoDbWriteException(string message)
            : base(message) { }

        public MongoDbWriteException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}