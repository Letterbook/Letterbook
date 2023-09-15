namespace Letterbook.Core.Exceptions;

public class DuplicateException : Exception
{
    public DuplicateException()
    {
    }

    public DuplicateException(string? message) : base(message)
    {
    }

    public DuplicateException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}