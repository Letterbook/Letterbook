using System.Net;
using Letterbook.Core.Exceptions;
using Letterbook.Core.Extensions;
using nietras.SeparatedValues;

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

	public static List<Peer> ParseLetterbook(string csv)
	{
		throw new NotImplementedException();
		// var now = DateTimeOffset.UtcNow;
		// using var reader = Sep.Reader().FromText(csv);
		// foreach (var required in Enum.GetValues<Restrictions>().Except([Restrictions.None]))
		// {
		// 	if (!reader.Header.ColNames.Contains(required.ToString()))
		// 		throw CoreException.InvalidRequest($"Missing required header {required}");
		// }
	}

	public static List<Peer> ParseMastodon(string csv)
	{
		var now = DateTimeOffset.UtcNow;
		using var reader = Sep.Reader().FromText(csv);
		foreach (var required in MastodonHeaders.All)
		{
			if (!Enumerable.Contains<string>(reader.Header.ColNames, required))
				throw CoreException.InvalidRequest($"Missing required header {required}");
		}

		var peers = new List<Peer>();
		foreach (var row in reader)
		{
			var peer = new Peer(row[MastodonHeaders.Domain].ToString());
			switch (row[MastodonHeaders.Severity].ToString())
			{
				case "silence":
					peer.Restrictions.Add(Models.Restrictions.LimitDiscovery, DateTimeOffset.MaxValue);
					peer.Restrictions.Add(Models.Restrictions.Warn, DateTimeOffset.MaxValue);
					break;
				case "suspend":
				default:
					peer.Restrictions.Add(Models.Restrictions.Defederate, DateTimeOffset.MaxValue);
					break;
			}

			if (row[MastodonHeaders.RejectMedia].TryParse<bool>(out var rejectMedia) && rejectMedia)
			{
				peer.Restrictions.Add(Models.Restrictions.DenyAttachments, DateTimeOffset.MaxValue);
			}

			if (row[MastodonHeaders.RejectReports].TryParse<bool>(out var rejectReports) && rejectReports)
			{
				peer.Restrictions.Add(Models.Restrictions.DenyReports, DateTimeOffset.MaxValue);
			}

			if (row[MastodonHeaders.Obfuscate].TryParse<bool>(out var obfuscate) && !obfuscate)
				peer.PublicRemark = row[MastodonHeaders.PublicComment].ToString();

			peer.PrivateComment = $"Imported at {now}";
			peers.Add(peer);
		}

		return peers;
	}
}

public static class MastodonHeaders
{
	public static string[] All = [Domain, Severity, RejectMedia, RejectReports, PublicComment, Obfuscate];

	public static string Domain => "#domain";
	public static string Severity => "#severity";
	public static string RejectMedia => "#reject_media";
	public static string RejectReports => "#reject_reports";
	public static string PublicComment => "#public_comment";
	public static string Obfuscate => "#obfuscate";
}
