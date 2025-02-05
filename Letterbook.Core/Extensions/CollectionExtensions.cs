using System.Linq.Expressions;
using Letterbook.Generators;

namespace Letterbook.Core.Extensions;

public static class CollectionExtensions
{
	/// <summary>
	/// Uses IEquatable comparisons to return a new Collection which has items that are equivalent to the replacement collection,
	/// but reusing any equivalent objects from the source collection.
	///
	/// The resulting collection is the same size and has the same values as the replacement, but retains reference equality to the
	/// source, where there was overlap.
	/// </summary>
	/// <remarks>
	/// This ensures that values are added and removed correctly, while preserving the reference equality for the source values.
	/// </remarks>
	/// <param name="source"></param>
	/// <param name="replacement"></param>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public static ICollection<T> ReplaceWith<T>(this ICollection<T> source, ICollection<T> replacement)
	{
		var result = source.ToHashSet();
		result.UnionWith(replacement);
		result.IntersectWith(replacement);

		return result;
	}

	/// <summary>
	/// Uses IEquatable comparisons to return a new Collection which is equivalent to the source collection, but has intersecting
	/// members taken from the replacement collection.
	///
	/// The resulting collection is the same size and has the same values as the source, but has reference equality to the
	/// replacement, where there was overlap.
	/// </summary>
	/// <remarks>
	/// This ensures that values are added and removed correctly, while preserving the reference equality for the source values.
	/// </remarks>
	/// <param name="source"></param>
	/// <param name="from"></param>
	public static ICollection<T> ReplaceFrom<T>(this ICollection<T> source, ICollection<T> from)
	{
		var result = from.ToHashSet();
		result.IntersectWith(source);
		result.UnionWith(source);

		return result;
	}

	public static ICollection<T> ReplaceFrom<T>(this ICollection<T> source, ICollection<T> from, IEqualityComparer<T> compare)
	{
		var result = from.Intersect(source, compare).Union(source, compare);
		return result.ToHashSet();
	}

	public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source) where T : class =>
		source.Where<T?>(TestNotNull)!;

	public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source) where T : struct =>
		source.Where(x => x.HasValue).Select(x => x!.Value);

	private static readonly Func<object?, bool> TestNotNull = x => x is not null;

	public static IEnumerable<T> Converge<T, TId>(this IEnumerable<T> source, Dictionary<TId, T> values, Func<T, TId> selector) where TId : notnull where T : class
	{
		return source.Select(each => values.GetValueOrDefault(selector(each))).WhereNotNull();
	}
}