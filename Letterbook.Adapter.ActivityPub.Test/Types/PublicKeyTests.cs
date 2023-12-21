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

        const string expectedJson = "https://w3id.org/security/v1";
        Assert.Contains(expectedJson, contextJson);
    }

    [Fact]
    public void ItShould_ExcludeASType()
    {
        var pubKey = new PublicKey
        {
            Id = "https://example.com/pubkey",
            PublicKeyPem = "key"
        };
        
        var contextJson = _serializer.SerializeToElement(pubKey);

        Assert.False(contextJson.TryGetProperty("type", out _));
    }
}