namespace Letterbook.Core.Exceptions;

public abstract class AdapterException : CoreException
{
    protected AdapterException(string? message) : base(message)
    {
    }

    protected AdapterException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}