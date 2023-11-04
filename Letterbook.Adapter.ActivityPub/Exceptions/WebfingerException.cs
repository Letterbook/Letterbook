using System.Net;
using System.Runtime.CompilerServices;
using Letterbook.Core.Exceptions;

namespace Letterbook.Adapter.ActivityPub.Exceptions;

public class WebfingerException : CoreException
{
    public WebfingerException(string? message) : base(message)
    {
    }

    public WebfingerException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
    
    public static WebfingerException InvalidQuery(Uri resource, Exception? innerEx = null,
        [CallerMemberName] string name="",
        [CallerFilePath] string path="",
        [CallerLineNumber] int line=-1)
    {
        var ex = new WebfingerException("Invalid Webfinger resource", innerEx)
        {
            Source = FormatSource(path, name, line),
        };
        ex.HResult |= (int)ErrorCodes.InvalidRequest;
        ex.Data["Resource"] = resource;

        return ex;
    }
}