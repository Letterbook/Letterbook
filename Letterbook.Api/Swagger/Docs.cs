using Swashbuckle.AspNetCore.SwaggerUI;

namespace Letterbook.Api.Swagger;

public static class Docs
{
	public const string LetterbookV1 = "letterbook-v1";
	public const string MastodonV1 = "mastodon-v1";
	public const string ActivityPubV1 = "activity-pub-v1";
	public static UrlDescriptor LetterbookV1Desc = new() { Name = LetterbookV1, Url = $"{LetterbookV1}/swagger.json" };
	public static UrlDescriptor MastodonV1Desc = new() { Name = MastodonV1, Url = $"{MastodonV1}/swagger.json" };
	public static UrlDescriptor ActivityPubV1Desc = new() { Name = ActivityPubV1, Url = $"{ActivityPubV1}/swagger.json" };
}