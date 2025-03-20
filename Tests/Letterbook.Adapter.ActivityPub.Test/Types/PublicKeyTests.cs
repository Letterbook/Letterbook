using ActivityPub.Types.Conversion;
using Letterbook.Adapter.ActivityPub.Types;
using Letterbook.Core.Tests.Fixtures;

namespace Letterbook.Adapter.ActivityPub.Test.Types;

public class PublicKeyTests : IClassFixture<JsonLdSerializerFixture>
{
	private readonly IJsonLdSerializer _serializer;

	public PublicKeyTests(JsonLdSerializerFixture fixture)
	{
		fixture.WriteIndented = false;
		_serializer = fixture.JsonLdSerializer;
	}

	[Fact]
	public void ItShouldDeserialize()
	{
		var json = """
                   {
                   "@context": [
                     "https://www.w3.org/ns/activitystreams",
                     "https://w3id.org/security/v1"
                   ],
                   "type": "Person",
                   "inbox": "https://example.com/inbox",
                   "outbox": "https://example.com/outbox",
                   "publicKey": {
                     "@context": [
                       "https://www.w3.org/ns/activitystreams",
                       "https://w3id.org/security/v1"
                     ],
                     "id": "https://example.com/key",
                     "publicKeyPem": "some key",
                     "owner": "https://example.com/owner"
                     }
                   }
                   """;
		var actual = _serializer.Deserialize<ProfileActor>(json);

		Assert.NotNull(actual);
		Assert.NotEmpty(actual.PublicKeys);
		Assert.Equal("https://example.com/key", actual.PublicKeys.First().Id);
	}

	[Fact(Skip = "Needs APSharp#152")]
	public void ItShould_IncludeJsonLdContext()
	{
		var pubKey = new PublicKey
		{
			Id = "https://example.com/pubkey",
			Owner = default!,
			PublicKeyPem = "key"
		};

		var contextJson = _serializer
			.SerializeToElement(pubKey)
			.GetProperty("@context")
			.ToString();

		const string expectedJson = "https://w3id.org/security/v1";
		Assert.Contains(expectedJson, contextJson);
	}

	[Fact]
	public void ItShould_ExcludeASType()
	{
		var pubKey = new PublicKey
		{
			Id = "https://example.com/pubkey",
			Owner = default!,
			PublicKeyPem = "key"
		};

		var contextJson = _serializer.SerializeToElement(pubKey);

		Assert.False(contextJson.TryGetProperty("type", out _));
	}
}