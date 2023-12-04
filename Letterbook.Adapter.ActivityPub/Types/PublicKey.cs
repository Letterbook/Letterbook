using System.Text.Json.Serialization;
using ActivityPub.Types;
using ActivityPub.Types.AS;
using ActivityPub.Types.Util;

namespace Letterbook.Adapter.ActivityPub.Types;

public class PublicKey : ASType, IASModel<PublicKey, PublicKeyEntity>
{
    private PublicKeyEntity Entity { get; }

    public PublicKey() => Entity = new PublicKeyEntity
    {
        Owner = default!,
        PublicKeyPem = default!,
        Id = default!
    };
    
    public PublicKey(TypeMap typeMap) : base(typeMap) => Entity = TypeMap.AsEntity<PublicKeyEntity>();

    public new string Id
    {
        get => Entity.Id;
        set => Entity.Id = value;
    }
    
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

    public static PublicKey FromGraph(TypeMap typeMap)
    {
        throw new NotImplementedException();
    }
}

public sealed class PublicKeyEntity : ASEntity<PublicKey, PublicKeyEntity>
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }
    
    [JsonPropertyName("owner")]
    public required Linkable<APActor> Owner { get; set; }
    
    [JsonPropertyName("publicKeyPem")]
    public required string PublicKeyPem { get; set; }
}