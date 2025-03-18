using System.Net.Http.Headers;

namespace Letterbook.Core;

public static class Constants
{
	public const string ActivityPubAccept = @"application/ld+json; profile=""https://www.w3.org/ns/activitystreams""";
	public const string ActivityPubPublicCollection = "https://www.w3.org/ns/activitystreams#Public";
	public const string LetterbookLocalCollection = "https://ns.letterbookhq.com#Local";
	public static readonly MediaTypeHeaderValue LdJsonHeader = MediaTypeHeaderValue.Parse(Constants.ActivityPubAccept);
}