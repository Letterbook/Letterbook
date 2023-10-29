using System.Net;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using Letterbook.Core.Exceptions;
using Letterbook.Core.Extensions;
using NSign;

namespace Letterbook.Adapter.ActivityPub.Exceptions;

public class VerifierException : CoreException
{
    public VerifierException(string? message) : base(message)
    {
    }

    public VerifierException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
    
    public static VerifierException NoSignatures(Exception? innerEx = null,
        [CallerMemberName] string name="",
        [CallerFilePath] string path="",
        [CallerLineNumber] int line=-1)
    {
        var ex = new VerifierException("The request was not signed", innerEx)
        {
            Source = FormatSource(path, name, line),
        };
        ex.HResult |= (int)ErrorCodes.PermissionDenied;

        return ex;
    }
    
    public static VerifierException NoValidSignatures(HttpRequestHeaders headers, Exception? innerEx = null,
        [CallerMemberName] string name="",
        [CallerFilePath] string path="",
        [CallerLineNumber] int line=-1)
    {
        var ex = new VerifierException("None of the provided signatures is valid", innerEx)
        {
            Source = FormatSource(path, name, line),
        };
        ex.HResult |= (int)ErrorCodes.PermissionDenied.With(ErrorCodes.InvalidRequest);
        if (headers.TryGetValues(Constants.Headers.Signature, out var signatures))
            ex.Data["Signature Header"] = signatures;
        if (headers.TryGetValues(Constants.Headers.SignatureInput, out var inputs))
            ex.Data["Signature-Input Header"] = inputs;
        if (headers.TryGetValues("User-Agent", out var userAgent))
            ex.Data["User-Agent Header"] = userAgent;

        return ex;
    }
}