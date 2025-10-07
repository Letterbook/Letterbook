using System.Linq.Expressions;
using Letterbook.Core.Models;
using Letterbook.Generators;

namespace Letterbook.Core.Extensions;

public static class CollectionExtensions
{
	private static readonly Func<object?, bool> TestNotNull = x => x is not null;

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


	/// <summary>
	/// Reproduce an IEnumerable, with equivalent elements taken from the values dictionary by matching a selector function.
	/// Values in the source IEnumerable which have no equivalent are excluded.
	/// </summary>
	/// <param name="source"></param>
	/// <param name="values"></param>
	/// <param name="selector">A function that returns a lookup key for each element of the source IEnumerable, to retrieve a value from the
	/// values dictionary</param>
	/// <typeparam name="T">The type of the source and resultant IEnumerable</typeparam>
	/// <typeparam name="TKey">The key type of the values dictionary</typeparam>
	/// <returns>The subset of the objects in the values dictionary which are equivalent to the elements in the source</returns>
	public static IEnumerable<T> Converge<T, TKey>(this IEnumerable<T> source, Dictionary<TKey, T> values, Func<T, TKey> selector) where TKey : notnull where T : class
	{
		return source.Select(each => values.GetValueOrDefault(selector(each))).WhereNotNull();
	}

	/// <summary>
	/// Try to redeem the code for any (and every) known occurence
	/// </summary>
	/// <remarks>
	/// In the event of a collision between invite codes, there's no way to know which instance a redemption should count toward.
	/// So, we count it against all of them. If any of them succeeds, then we consider the code valid.
	/// Collisions should be rare, and collisions between simultaneously valid codes should be exceedingly rare. So this is acceptable.
	/// </remarks>
	/// <param name="source"></param>
	/// <param name="account"></param>
	/// <returns>True if the code was redeemable, false otherwise</returns>
	public static bool TryRedeemAny(this IEnumerable<InviteCode> source, Account account)
	{
		return source.Select(code => code.TryRedeem(account)).Aggregate(false, (acc, each) => each || acc);
	}
}