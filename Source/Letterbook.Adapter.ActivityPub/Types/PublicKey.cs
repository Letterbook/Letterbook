using System.Text.Json.Serialization;
using ActivityPub.Types.AS;
using ActivityPub.Types.Util;

namespace Letterbook.Adapter.ActivityPub.Types;

public class PublicKey
{
	[JsonPropertyName("id")]
	public required string Id { get; set; }

	[JsonPropertyName("owner")]
	public required Linkable<APActor> Owner { get; set; }

	[JsonPropertyName("publicKeyPem")]
	public required string PublicKeyPem { get; set; }
}