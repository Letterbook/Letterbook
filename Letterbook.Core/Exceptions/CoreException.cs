using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Letterbook.Core.Exceptions;

public class CoreException : Exception
{
    /// <summary>
    /// While you *can* construct your own CoreException, it's better to use a factory method, from below.
    /// Exceptions form part of the mechanism to report error messages, to both users and operators. We can have better
    /// and more helpful error messages if they're consistent. The factories help with that.
    /// </summary>
    /// <param name="message"></param>
    public CoreException(string? message) : base(message)
    {
        HResult = 0;
    }

    public CoreException(string? message, Exception? innerException) : base(message, innerException)
    {
        HResult = 0;
    }
    
    public bool Flagged(ErrorCodes code)
    {
        return ((ErrorCodes)HResult & code) == code;
    }

    // Indicates creating a new resource would conflict with an existing resource
    public static CoreException Duplicate(string message, object id, Exception? innerEx = null,
        [CallerMemberName] string name="",
        [CallerFilePath] string path="",
        [CallerLineNumber] int line=-1)
    {
        var ex = new CoreException(message, innerEx)
        {
            Source = FormatSource(path, name, line),
        };
        ex.HResult |= (int)ErrorCodes.DuplicateEntry;
        ex.Data.Add("Id", id);

        return ex;
    }

    // Indicates that the request is not semantically valid or violates some application constraint
    public static CoreException InvalidRequest(string message, IDictionary<string, object>? details = null, Exception? innerEx = null,
        [CallerMemberName] string name = "",
        [CallerFilePath] string path = "",
        [CallerLineNumber] int line = -1)
    {
        var ex = new CoreException(message, innerEx)
        {
            Source = FormatSource(path, name, line),
        };
        ex.HResult |= (int)ErrorCodes.InvalidRequest;
        if (details == null) return ex;
        
        foreach (var detail in details)
        {
            ex.Data.Add(detail.Key, detail.Value);
        }

        return ex;
    }

    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    public static CoreException InvalidRequest(string message, string key, object value, Exception? innerEx = null,
        [CallerMemberName] string name = "",
        [CallerFilePath] string path = "",
        [CallerLineNumber] int line = -1)
    {
        var details = new Dictionary<string, object>();
        details.Add(key, value);
        return InvalidRequest(message, details, innerEx, name, path, line);
    }

    // Indicates the server has been asked to modify an object it does not control
    public static CoreException WrongAuthority(string message, Uri target, Uri? source = null, Exception? innerEx = null,
        [CallerMemberName] string name = "",
        [CallerFilePath] string path = "",
        [CallerLineNumber] int line = -1)
    {
        var ex = new CoreException(message, innerEx)
        {
            Source = FormatSource(path, name, line),
        };
        ex.HResult |= (int)ErrorCodes.WrongAuthority;
        ex.Data.Add("Target", target);
        if(source != null) ex.Data.Add("Source", source);

        return ex;
    }
    
    // Indicates some required data could not be found
    public static CoreException MissingData(string message, Type type, object? id, Exception? innerEx = null,
        [CallerMemberName] string name = "",
        [CallerFilePath] string path = "",
        [CallerLineNumber] int line = -1)
    {
        var ex = new CoreException(message, innerEx)
        {
            Source = FormatSource(path, name, line),
        };
        ex.HResult |= (int)ErrorCodes.MissingData;
        ex.Data.Add(type.ToString(), id);

        return ex;
    }

    public static CoreException InternalError(string message, Exception? innerEx = null,
        [CallerMemberName] string name = "",
        [CallerFilePath] string path = "",
        [CallerLineNumber] int line = -1)
    {
        var ex = new CoreException(message, innerEx)
        {
            Source = FormatSource(path, name, line),
        };
        ex.HResult |= (int)ErrorCodes.InternalError;

        return ex;
    }

    protected static string FormatSource(string path, string name, int line)
    {
        return $"{path} [{name}:{line}]";
    }
}