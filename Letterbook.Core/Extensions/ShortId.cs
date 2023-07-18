using Letterbook.Core.Models;

namespace Letterbook.Core.Extensions;

public static class ShortId
{
    public static string ToShortId(this Guid g)
    {
        var id = Convert.ToBase64String(g.ToByteArray());
        return id.Replace("=", "").Replace("+", "-").Replace("/", "_");
    }

    public static Guid ToGuid(string shortId)
    {
        var id = shortId.Replace("-", "+").Replace("_", "/").PadRight(24, '=');
        return new Guid(Convert.FromBase64String(id));
    }

    public static string NewShortId()
    {
        return Guid.NewGuid().ToShortId();
    }
}