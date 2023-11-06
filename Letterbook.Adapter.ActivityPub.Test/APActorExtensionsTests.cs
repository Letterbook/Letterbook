using ActivityPub.Types.AS;
using ActivityPub.Types.Conversion;
using ActivityPub.Types.Internal;
using Microsoft.Extensions.Options;

namespace Letterbook.Adapter.ActivityPub.Test;

public class APActorExtensionsTests
{
    private JsonLdSerializer _serializer => new JsonLdSerializer(Options.Create(new JsonLdSerializerOptions()), new ASTypeInfoCache());
    
    [Fact]
    public void TestSerialize()
    {
        var actor = new APActorExtensions()
        {
            Inbox = "http://letterbook.example/actor/inbox",
            Outbox = "http://letterbook.example/actor/outbox",
            PublicKey = new PublicKey()
            {
                Id = "http://letterbook.example/actor/keys/0",
                PublicKeyPem = @"-----BEGIN PUBLIC KEY-----\nBase64KeyValues\n-----END PUBLIC KEY-----\n"
            }
        };

        var json = _serializer.SerializeToElement(actor);
        
        Assert.True(json.TryGetProperty("publicKey", out var publicKey), "missing publicKey");
        Assert.True(publicKey.TryGetProperty("publicKeyPem", out var publicKeyPem), "missing publicKeyPem");
        Assert.Equal(actor.PublicKey.PublicKeyPem, publicKeyPem.GetString());
        Assert.True(json.TryGetProperty("type", out var type));
        
        // It should be some kind of Actor type. How should that be set?
        Assert.NotEqual("Object", type.GetString()); // Fails
        // Also, the publicKey object should _not_ have a `type`, but it does. Should it inherit from something else?
    }

    [Fact]
    public void TestDeserialize()
    {
        var json = """
                   {
                     "type": "Person",
                     "@context": "https://www.w3.org/ns/activitystreams",
                     "inbox": "http://letterbook.example/actor/inbox",
                     "outbox": "http://letterbook.example/actor/outbox",
                     "publicKey": {
                       "@context": "https://www.w3.org/ns/activitystreams",
                       "id": "http://letterbook.example/actor/keys/0",
                       "publicKeyPem": "-----BEGIN PUBLIC KEY-----\nBase64KeyValues\\n-----END PUBLIC KEY-----\n"
                     }
                   }
                   """;
        // How would I deserialize an object whose derived type I don't know in advance? I also tried to deserialize to ASObject and APActor, both fail.
        var poco = _serializer.Deserialize<ASType>(json); // throws
        
        Assert.True(poco?.Is<APActorExtensions>());
        Assert.NotNull(poco?.As<APActorExtensions>().PublicKey);
        Assert.Equal(@"-----BEGIN PUBLIC KEY-----\nBase64KeyValues\\n-----END PUBLIC KEY-----\n", poco?.As<APActorExtensions>().PublicKey?.PublicKeyPem);
    }
}
