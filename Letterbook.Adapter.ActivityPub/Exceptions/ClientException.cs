using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Runtime.CompilerServices;
using Letterbook.Core.Exceptions;
using Letterbook.Core.Extensions;

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

    public static ClientException SignatureError(Guid keyId, string keyLabel, Exception? innerEx = null,
        [CallerMemberName] string name="",
        [CallerFilePath] string path="",
        [CallerLineNumber] int line=-1)
    {
        var ex = new ClientException("No private signing key available. This may be a remote profile.", innerEx)
        {
            Source = FormatSource(path, name, line),
        };
        ex.HResult |= (int)ErrorCodes.MissingData
            .With(ErrorCodes.PermissionDenied)
            .With(ErrorCodes.WrongAuthority);
        ex.Data["id"] = keyId;
        ex.Data["label"] = keyLabel;

        return ex;
    }

    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    public static ClientException SignatureError([CallerMemberName] string name = "",
        [CallerFilePath] string path = "",
        [CallerLineNumber] int line = -1) => SignatureError(Guid.Empty, null, null, name, path, line);
}