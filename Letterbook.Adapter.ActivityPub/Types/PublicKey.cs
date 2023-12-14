using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using ActivityPub.Types;
using ActivityPub.Types.AS;
using ActivityPub.Types.Util;

namespace Letterbook.Adapter.ActivityPub.Types;

public class PublicKey : ASObject, IASModel<PublicKey, PublicKeyEntity, ASObject>
{
    private PublicKeyEntity Entity { get; }

    public PublicKey() : this(new TypeMap()) {}
    
    public PublicKey(TypeMap typeMap) : base(typeMap)
        => Entity = TypeMap.Extend<PublicKeyEntity>();
    
    public PublicKey(ASType existingGraph) : this(existingGraph.TypeMap) {}
    
    [SetsRequiredMembers]
    public PublicKey(TypeMap typeMap, PublicKeyEntity? entity) : base(typeMap, null)
    {
        Entity = entity ?? typeMap.AsEntity<PublicKeyEntity>();
        Id = Entity.Id ?? throw new ArgumentException($"The provided entity is invalid - required {nameof(PublicKeyEntity.Id)} property is missing");
        PublicKeyPem = Entity.PublicKeyPem ?? throw new ArgumentException($"The provided entity is invalid - required {nameof(PublicKeyEntity.PublicKeyPem)} property is missing");
    }

    static PublicKey IASModel<PublicKey>.FromGraph(TypeMap typeMap) => new(typeMap, null);

    public required new string Id
    {
        get => Entity.Id!;
        set => Entity.Id = value;
    }
    
    public Linkable<APActor>? Owner
    {
        get => Entity.Owner;
        set => Entity.Owner = value;
    }

    public required string PublicKeyPem
    {
        get => Entity.PublicKeyPem!;
        set => Entity.PublicKeyPem = value;
    }
}

public sealed class PublicKeyEntity : ASEntity<PublicKey, PublicKeyEntity>
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    
    [JsonPropertyName("owner")]
    public Linkable<APActor>? Owner { get; set; }
    
    [JsonPropertyName("publicKeyPem")]
    public string? PublicKeyPem { get; set; }
}