using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using ActivityPub.Types;
using ActivityPub.Types.AS;
using ActivityPub.Types.Util;

namespace Letterbook.Adapter.ActivityPub.Types;

public class PublicKey
{
	// TODO: Restore the ASTyped version (check the git history)
	// Requires https://github.com/warriordog/ActivityPubSharp/issues/152
	[JsonPropertyName("id")]
	public required string Id { get; set; }

	[JsonPropertyName("owner")]
	public required Linkable<APActor> Owner { get; set; }

	[JsonPropertyName("publicKeyPem")]
	public required string PublicKeyPem { get; set; }
}