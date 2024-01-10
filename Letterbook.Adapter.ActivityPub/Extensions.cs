using System.Diagnostics.CodeAnalysis;
using ActivityPub.Types.AS;
using ActivityPub.Types.Util;

namespace Letterbook.Adapter.ActivityPub;

public static class Extensions
{
    public static bool TryGetId(this Linkable<ASObject> linkable, [NotNullWhen(true)]out Uri? id)
    {
        if (linkable.TryGetValue(out var value) && value.Id != null)
        {
            id = new Uri(value.Id);
            return true;
        }
        
        if (linkable.TryGetLink(out var link))
        {
            id = link.HRef;
            return true;
        }

        id = null;
        return false;
    }
    
    public static bool TryGetId(this ASObject aso, [NotNullWhen(true)]out Uri? id)
    {
        if (aso.Id != null)
        {
            id = new Uri(aso.Id);
            return true;
        }

        id = default;
        return false;
    }

    public static bool TryGetId(this ASType asType, [NotNullWhen(true)] out Uri? id)
    {
        if (asType.Id != null)
        {
            id = new Uri(asType.Id);
            return true;
        }

        if (asType.Is<ASLink>(out var link))
        {
            id = link.HRef;
            return true;
        }

        id = default;
        return false;
    }

    public static string NotNull(params string?[] args) =>
        args.FirstOrDefault(s => s is not null)
        ?? throw new ArgumentOutOfRangeException(nameof(args), "All of the attempted values were null");

    public static IEnumerable<Uri> SelectIds(this IEnumerable<ASObject> objects) =>
        objects.Select(o => o.Id).OfType<string>().Select(s => new Uri(s));

    public static IEnumerable<Uri> SelectIds(this IEnumerable<ASLink> links) =>
        links.Select(o => o.HRef.Uri);
}