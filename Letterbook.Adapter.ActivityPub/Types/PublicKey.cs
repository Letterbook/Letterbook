using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using ActivityPub.Types;
using ActivityPub.Types.AS;
using ActivityPub.Types.Util;

namespace Letterbook.Adapter.ActivityPub.Types;

public class PublicKey : ASType, IASModel<PublicKey, PublicKeyEntity, ASType>
{
    // { "sec": "https://w3id.org/security/v1#" }
    public static IJsonLDContext DefiningContext { get; } = new JsonLDContext
    {
        new("https://w3id.org/security/v1")
    };

    private PublicKeyEntity Entity { get; }

    public PublicKey() => Entity = TypeMap.Extend<PublicKey, PublicKeyEntity>();
    
    public PublicKey(TypeMap typeMap, bool isExtending = true) : base(typeMap, false)
        => Entity = TypeMap.ProjectTo<PublicKey, PublicKeyEntity>(isExtending);
    
    public PublicKey(ASType existingGraph) : this(existingGraph.TypeMap) {}
    
    [SetsRequiredMembers]
    public PublicKey(TypeMap typeMap, PublicKeyEntity? entity) : base(typeMap, null)
    {
        Entity = entity ?? typeMap.AsEntity<PublicKey, PublicKeyEntity>();
        Id = Entity.Id ?? throw new ArgumentException($"The provided entity is invalid - required {nameof(PublicKeyEntity.Id)} property is missing");
        PublicKeyPem = Entity.PublicKeyPem ?? throw new ArgumentException($"The provided entity is invalid - required {nameof(PublicKeyEntity.PublicKeyPem)} property is missing");
    }

    static PublicKey IASModel<PublicKey>.FromGraph(TypeMap typeMap) => new(typeMap, null);

    public new required string Id
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