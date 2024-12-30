using System.Net;
using Letterbook.Core.Extensions;

namespace Letterbook.Core.Models;

public class Peer
{
	public string Hostname { get; set; }
	public string Authority { get; set; }
	public IDictionary<Restrictions, DateTimeOffset> Restrictions { get; set; } = new Dictionary<Restrictions, DateTimeOffset>();

	public Peer(Uri address)
	{
		Hostname = address.Authority;
		Authority = address.GetAuthority();
	}
}