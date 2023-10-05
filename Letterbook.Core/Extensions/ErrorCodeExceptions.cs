using Letterbook.Core.Exceptions;

namespace Letterbook.Core.Extensions;

public static class ErrorCodeExceptions
{
    public static ErrorCodes With(this ErrorCodes self, int flag)
    {
        return (ErrorCodes)((int)self | flag);
    }
    
    public static bool Flagged(this ErrorCodes self, int flag)
    {
        return ((int)self & flag) == flag;
    }
}