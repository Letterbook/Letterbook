namespace Letterbook.Core.Exceptions;

public class RateLimitException : Exception
{
    public DateTimeOffset Expiration { get; set; }
    
    public RateLimitException(string? message, DateTimeOffset expiration) : base(message)
    {
        Expiration = expiration;
    }

    public RateLimitException(string? message, Exception? innerException, DateTimeOffset expiration) : base(message, innerException)
    {
        Expiration = expiration;
    }
}