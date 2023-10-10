using System.Net;
using System.Runtime.CompilerServices;
using Letterbook.Core.Exceptions;

namespace Letterbook.Adapter.ActivityPub.Exceptions;

public class ClientException : AdapterException
{
    public ClientException(string? message) : base(message)
    {
    }

    public ClientException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
    
    public static ClientException RemoteHostError(HttpStatusCode code, string? message = null, Exception? innerEx = null,
        [CallerMemberName] string name="",
        [CallerFilePath] string path="",
        [CallerLineNumber] int line=-1)
    {
        var ex = new ClientException(message, innerEx)
        {
            Source = FormatSource(path, name, line),
        };
        ex.HResult |= (int)ErrorCodes.PeerError;
        ex.Data["Error Code"] = code;

        return ex;
    }

    public static ClientException RequestError(HttpStatusCode code, string? message = null, string? body = null, Exception? innerEx = null,
        [CallerMemberName] string name="",
        [CallerFilePath] string path="",
        [CallerLineNumber] int line=-1)
    {
        var ex = new ClientException(message, innerEx)
        {
            Source = FormatSource(path, name, line),
        };
        ex.HResult |= (int)ErrorCodes.InvalidRequest;
        if (code == HttpStatusCode.Forbidden) ex.HResult |= (int)ErrorCodes.PermissionDenied;
        ex.Data["Error Code"] = code;

        return ex;
    }
}