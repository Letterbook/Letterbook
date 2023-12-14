using ActivityPub.Types.Conversion;
using Letterbook.Adapter.ActivityPub.Test.Fixtures;
using Letterbook.Adapter.ActivityPub.Types;

namespace Letterbook.Adapter.ActivityPub.Test.Types;

public class PublicKeyTests : IClassFixture<JsonLdSerializerFixture>
{
    private readonly IJsonLdSerializer _serializer;

    public PublicKeyTests(JsonLdSerializerFixture fixture)
    {
        _serializer = fixture.JsonLdSerializer;
        _serializer.SerializerOptions.WriteIndented = false;
    }

    [Fact]
    public void ItShould_IncludeJsonLdContext()
    {
        var pubKey = new PublicKey
        {
            Id = "https://example.com/pubkey",
            PublicKeyPem = "key"
        };

        var contextJson = _serializer
            .SerializeToElement(pubKey)
            .GetProperty("@context")
            .ToString();

        const string ExpectedJson = """{"sec":"https://w3id.org/security/v1#"}""";
        Assert.Contains(ExpectedJson, contextJson);
    }
}