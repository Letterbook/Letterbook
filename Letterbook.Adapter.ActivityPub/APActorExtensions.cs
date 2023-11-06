using System.Text.Json.Serialization;
using ActivityPub.Types;
using ActivityPub.Types.AS;
using ActivityPub.Types.Attributes;
using ActivityPub.Types.Util;

namespace Letterbook.Adapter.ActivityPub;

// The goal here is to extend AP actor with the PublicKey property, and have it (de)serialize appropriately
// Am I on the right track with all this?
public class APActorExtensions : APActor
{
    private APActorExtensionsEntity Entity { get; }

    public APActorExtensions() => Entity = new APActorExtensionsEntity { TypeMap = TypeMap };
    public APActorExtensions(TypeMap typeMap) : base(typeMap) => Entity = TypeMap.AsEntity<APActorExtensionsEntity>();

    public PublicKey? PublicKey
    {
        get => Entity.PublicKey;
        set => Entity.PublicKey = value!;
    }
}

[APConvertible("what goes here?")] // Is this even needed? Do I need one for each Actor type?
// Is this right?
[ImpliesOtherEntity(typeof(APActorEntity))]
public sealed class APActorExtensionsEntity : ASEntity<APActorExtensions>
{
    // Does this have to duplicate all of the properties of APActorEntity?
    
    [JsonPropertyName("publicKey")]
    public PublicKey PublicKey { get; set; }
}


// PublicKeys

public class PublicKey : ASObject
{
    private PublicKeyEntity Entity { get; }

    public PublicKey() => Entity = new PublicKeyEntity()
    {
        TypeMap = TypeMap,
        Owner = default!,
        PublicKeyPem = default!
    };
    
    public PublicKey(TypeMap typeMap) : base(typeMap) => Entity = TypeMap.AsEntity<PublicKeyEntity>();

    public Linkable<APActor> Owner
    {
        get => Entity.Owner;
        set => Entity.Owner = value;
    }

    public string PublicKeyPem
    {
        get => Entity.PublicKeyPem;
        set => Entity.PublicKeyPem = value;
    }
}

// PublicKey isn't part of a type set with anything else, so it probably doesn't need ImpliesOtherEntity, right?
public sealed class PublicKeyEntity : ASEntity<PublicKey>
{
    [JsonPropertyName("owner")]
    public required Linkable<APActor> Owner { get; set; }
    
    [JsonPropertyName("publicKeyPem")]
    public required string PublicKeyPem { get; set; }
}
