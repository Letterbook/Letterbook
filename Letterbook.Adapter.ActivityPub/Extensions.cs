using System.Diagnostics.CodeAnalysis;
using ActivityPub.Types.AS;
using ActivityPub.Types.AS.Collection;
using ActivityPub.Types.Util;

namespace Letterbook.Adapter.ActivityPub;

public static class Extensions
{
	private static readonly Func<object?, bool> TestNotNull = x => x is not null;

	public static bool TryGetId(this Linkable<ASObject> linkable, [NotNullWhen(true)] out Uri? id)
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

	public static bool TryGetId(this Linkable<ASCollection> linkable, [NotNullWhen(true)] out Uri? id)
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


	public static bool TryGetId(this ASObject aso, [NotNullWhen(true)] out Uri? id)
	{
		if (aso.Id != null)
		{
			id = new Uri(aso.Id);
			return true;
		}

		id = default;
		return false;
	}

	public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source) where T : class =>
		source.Where((Func<T?, bool>)TestNotNull)!;

	public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source) where T : struct =>
		source.Where(x => x.HasValue).Select(x => x!.Value);

	public static string NotNull(params string?[] args) =>
		args.FirstOrDefault(s => TestNotNull(s))
		?? throw new ArgumentOutOfRangeException(nameof(args), "All of the attempted values were null");

	public static IEnumerable<Uri> SelectIds(this IEnumerable<ASObject> objects) =>
		objects.Select(o => o.Id).WhereNotNull().Select(s => new Uri(s));

	public static IEnumerable<Uri> SelectIds(this IEnumerable<ASLink> links) =>
		links.Select(o => o.HRef.Uri);
}