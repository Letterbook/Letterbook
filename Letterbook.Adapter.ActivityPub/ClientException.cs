using System.Runtime.CompilerServices;
using Letterbook.Core.Exceptions;

namespace Letterbook.Adapter.ActivityPub;

public class ClientException : AdapterException
{
    public ClientException(string? message) : base(message)
    {
    }

    public ClientException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    // Indicates the client action can only be performed on behalf of a Profile
    public static ClientException ProfileRequired(string message, Exception? innerEx = null,
        [CallerMemberName] string name="",
        [CallerFilePath] string path="",
        [CallerLineNumber] int line=-1)
    {
        var ex = new ClientException(message, innerEx)
        {
            Source = FormatSource(path, name, line),
        };
        ex.HResult |= (int)ErrorCodes.DuplicateEntry;

        return ex;
    }
}