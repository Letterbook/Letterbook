using Letterbook.Core.Models;

namespace Letterbook.Core;

public class FediIdCompare : IEqualityComparer<IFederated>
{
	private FediIdCompare() { }
	public static FediIdCompare Instance { get; } = new();

	public bool Equals(IFederated? x, IFederated? y)
	{
		if (ReferenceEquals(x, y)) return true;
		if (x is null) return false;
		if (y is null) return false;
		if (x.GetType() != y.GetType()) return false;
		return x.FediId.Equals(y.FediId);
	}

	public int GetHashCode(IFederated obj)
	{
		return obj.FediId.GetHashCode();
	}
}