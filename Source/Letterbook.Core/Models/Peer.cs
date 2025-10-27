using System.Net;
using Letterbook.Core.Extensions;

namespace Letterbook.Core.Models;

public class Peer : IEquatable<Peer>
{
	public string Hostname { get; set; }
	public string Authority { get; set; }
	public string? PublicRemark { get; set; }
	public string? PrivateComment { get; set; }
	public IDictionary<Restrictions, DateTimeOffset> Restrictions { get; set; } = new Dictionary<Restrictions, DateTimeOffset>();

	public Peer(Uri address)
	{
		Hostname = address.Authority;
		Authority = address.GetAuthority();
	}

	public Peer(string domain)
	{
		var uri = new Uri($"https://{domain}");
		Hostname = uri.Authority;
		Authority = uri.GetAuthority();
	}

	private Peer()
	{
		Hostname = default!;
		Authority = default!;
	}

	public bool Equals(Peer? other)
	{
		if (other is null) return false;
		if (ReferenceEquals(this, other)) return true;
		return Authority == other.Authority;
	}

	public override bool Equals(object? obj)
	{
		if (obj is null) return false;
		if (ReferenceEquals(this, obj)) return true;
		if (obj.GetType() != GetType()) return false;
		return Equals((Peer)obj);
	}

	public override int GetHashCode()
	{
		return Authority.GetHashCode();
	}
}