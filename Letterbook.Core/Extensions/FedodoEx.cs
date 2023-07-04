using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Fedodo.NuGet.ActivityPub.Model.CoreTypes;
using Fedodo.NuGet.ActivityPub.Model.JsonConverters.Model;
using Object = Fedodo.NuGet.ActivityPub.Model.CoreTypes.Object;

namespace Letterbook.Core.Extensions;

/// <summary>
/// ActivityPubExtensions and helpers
///
/// ActivityPub.Model makes practically everything nullable, which is incredibly annoying. Even moreso when those things
/// are an IEnumerable. These methods mostly just make it easy to use them with Linq, by creating an empty IEnumerable
/// instead of null.
///
/// The returned values are not guaranteed to actually exist on the object as expected, so they must never be used to
/// modify the containing object, and are thus immutable.
/// </summary>
public static class FedodoEx
{
    public static IImmutableList<T> GetObjects<T>(this TripleSet<T> value) where T : Object
    {
        return value.Objects?.ToImmutableList() ?? ImmutableList<T>.Empty;
    }
    
    public static IImmutableList<string> GetStringLinks<T>(this TripleSet<T> value) where T : Object
    {
        return value.StringLinks?.ToImmutableList() ?? ImmutableList<string>.Empty;
    }
    
    public static IImmutableList<Link> GetLinks<T>(this TripleSet<T> value) where T : Object
    {
        return value.Links?.ToImmutableList() ?? ImmutableList<Link>.Empty;
    }
    
    public static IImmutableList<T> HasDefault<T>(IEnumerable<T>? value) where T : Object
    {
        return value?.ToImmutableList() ?? ImmutableList<T>.Empty;
    }
}