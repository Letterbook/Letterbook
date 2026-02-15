using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Letterbook.Core.Extensions;

public static partial class UriExtensions
{
	[GeneratedRegex("^acct:(?=[^/])")]
	public static partial Regex MatchAcct();

	// match any string that begins with '@' and includes one or more characters other than '@'
	// I'm sure other software disagrees, but there should be no reason we can't support literally any unicode character as part of a
	// profile handle, except the special character @ which separates the username from the hostname
	[GeneratedRegex("""(?<=^@)[^@]+$""")]
	public static partial Regex MatchLocalHandle();

	public static string GetAuthority(this Uri uri) => uri.IsDefaultPort
		? string.Join('.', uri.Host.Split('.').Reverse())
		: string.Join('.', uri.Host.Split('.').Reverse()) + $":{uri.Port}";

	public static bool TryParseHandle(string query, [NotNullWhen(true)]out string? handle, [NotNullWhen(true)]out Uri? host)
	{
		query = MatchAcct().Replace(query, "");
		query = string.Join('@', query.Split('@', 2,
			StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));

		UriBuilder builder;
		try
		{
			builder = new UriBuilder(query);
		}
		catch (UriFormatException )
		{
			handle = null;
			host = null;
			return false;
		}

		if (string.IsNullOrEmpty(builder.UserName) ||
		    string.IsNullOrEmpty(builder.Host) ||
		    builder.Path != "/" ||
		    !string.IsNullOrEmpty(builder.Password) ||
		    !string.IsNullOrEmpty(builder.Query) ||
		    !builder.Uri.IsDefaultPort ||
		    builder.Uri.IsLoopback ||
		    builder.Uri.HostNameType != UriHostNameType.Dns)
		{
			handle = null;
			host = null;
			return false;
		}

		handle = builder.UserName;
		builder.Fragment = "";
		builder.Scheme = Uri.UriSchemeHttps;
		builder.Port = 443;
		builder.UserName = "";
		host = builder.Uri;
		return true;
	}

	public static bool TryParseLocalHandle(string query, [NotNullWhen(true)] out string? handle)
	{
		var matches = MatchLocalHandle().Matches(query);
		if (matches.Count > 0)
		{
			handle = matches[0].Value;
			return true;
		}

		handle = null;
		return false;
	}
}