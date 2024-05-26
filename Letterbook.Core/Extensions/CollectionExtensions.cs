namespace Letterbook.Core.Extensions;

public static class CollectionExtensions
{
	/// <summary>
	/// Uses IEquatable comparisons to return a new Collection which has items that are equivalent to the replacement collection,
	/// but reusing any equivalent objects from the source collection.
	///
	/// This ensures that values are added and removed correctly, while preserving the reference equality for the source values.
	/// </summary>
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
}