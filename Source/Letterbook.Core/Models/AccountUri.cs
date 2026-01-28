namespace Letterbook.Core.Models;

// https://datatracker.ietf.org/doc/html/rfc7565
public class AccountUri
{
	public string Scheme { get; set; } = "";
	public string User { get; set; } = "";
	public string Host { get; set; } = "";

	public static bool TryParse(string? text, out AccountUri? result)
	{
		if (Uri.TryCreate(text, UriKind.Absolute, out var uri))
		{
			var userAndHost = uri.AbsolutePath.Split('@');

			result = new AccountUri
			{
				Scheme = uri.Scheme,
				Host = userAndHost.Last(),
				User = userAndHost.First()
			};

			return true;
		}

		result = null;

		return false;
	}
}