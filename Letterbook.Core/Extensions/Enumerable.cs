
namespace Letterbook.Core.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<T> SelfOrDefault<T>(this IEnumerable<T>? value)
    {
        return value ?? Enumerable.Empty<T>();
    }
}