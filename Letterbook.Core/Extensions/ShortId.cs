using Letterbook.Core.Models;

namespace Letterbook.Core.Extensions;

public static class ShortId
{
    public static string ToShortId(this Guid g)
    {
        Span<byte> guidBytes = stackalloc byte[16];
        if (!g.TryWriteBytes(guidBytes))
        {
            // this should never happen
            throw new InvalidOperationException("Could not write Guid bytes");
        }

        Span<char> base64 = stackalloc char[24];
        if (!Convert.TryToBase64Chars(guidBytes, base64, out int _))
        {
            // this should never happen
            throw new InvalidOperationException("Could not convert to base64");
        }

        base64.Replace('+', '-');
        base64.Replace('/', '_');

        return new string(base64.TrimEnd('='));
    }

    public static Guid ToGuid(ReadOnlySpan<char> shortId)
    {
        if (shortId.Length != 22)
        {
            throw new ArgumentException("Invalid shortId", nameof(shortId));
        }

        Span<char> id = stackalloc char[24];
        shortId.CopyTo(id);
        id.Replace('-', '+');
        id.Replace('_', '/');
        id[^2..].Fill('=');

        Span<byte> guidBytes = stackalloc byte[16];
        if (!Convert.TryFromBase64Chars(id, guidBytes, out int _))
        {
            throw new ArgumentException("Invalid shortId", nameof(shortId));
        }

        return new Guid(guidBytes);
    }

    public static string NewShortId()
    {
        return Guid.NewGuid().ToShortId();
    }
}
