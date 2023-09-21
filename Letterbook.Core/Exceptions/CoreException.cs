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
    }

    public CoreException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    public static CoreException Duplicate(string message, object id, Exception? innerEx = null,
        [CallerMemberName] string name="",
        [CallerFilePath] string path="",
        [CallerLineNumber] int line=-1)
    {
        var ex = new CoreException(message, innerEx)
        {
            Source = $"{path} [{name}:{line}]"
        };
        ex.HResult += (int)ErrorCodes.DuplicateEntry;
        ex.Data.Add("Id", id);

        return ex;
    }

    public static CoreException Invalid(string message, IDictionary<string, object>? details = null, Exception? innerEx = null,
        [CallerMemberName] string name = "",
        [CallerFilePath] string path = "",
        [CallerLineNumber] int line = -1)
    {
        var ex = new CoreException(message, innerEx)
        {
            Source = $"{path} [{name}:{line}]"
        };
        ex.HResult += (int)ErrorCodes.InvalidRequest;
        if (details == null) return ex;
        
        foreach (var detail in details)
        {
            ex.Data.Add(detail.Key, detail.Value);
        }

        return ex;
    }

    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    public static CoreException Invalid(string message, string key, object value, Exception? innerEx = null,
        [CallerMemberName] string name = "",
        [CallerFilePath] string path = "",
        [CallerLineNumber] int line = -1)
    {
        var details = new Dictionary<string, object>();
        details.Add(key, value);
        return Invalid(message, details, innerEx, name, path, line);
    }
    
    public static CoreException Missing(string message, string key, Exception? innerEx = null,
        [CallerMemberName] string name = "",
        [CallerFilePath] string path = "",
        [CallerLineNumber] int line = -1)
    {
        var ex = new CoreException(message, innerEx)
        {
            Source = $"{path} [{name}:{line}]",
        };
        ex.HResult += (int)ErrorCodes.MissingData;
        ex.Data.Add("Key", key);

        return ex;
    }
}