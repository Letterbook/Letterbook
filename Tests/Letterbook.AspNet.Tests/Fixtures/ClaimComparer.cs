using System.Security.Claims;

namespace Letterbook.AspNet.Tests.Fixtures;

public class ClaimComparer : IEqualityComparer<Claim>
{
	public bool Equals(Claim? x, Claim? y)
	{
		if (ReferenceEquals(x, y)) return true;
		if (x is null) return false;
		if (y is null) return false;
		if (x.GetType() != y.GetType()) return false;
		return x.Type == y.Type && x.Value == y.Value && x.ValueType == y.ValueType;
	}

	public int GetHashCode(Claim obj)
	{
		return HashCode.Combine(obj.Type, obj.Value, obj.ValueType);
	}
}