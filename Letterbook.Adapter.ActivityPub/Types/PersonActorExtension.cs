using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using ActivityPub.Types;
using ActivityPub.Types.AS;
using ActivityPub.Types.AS.Extended.Actor;

// using Letterbook.Adapter.ActivityPub.Types;
// ReSharper disable InconsistentNaming

namespace Letterbook.Adapter.ActivityPub.Types;

public class PersonActorExtension : PersonActor, IASModel<PersonActorExtension, PersonActorExtensionsEntity, PersonActor>
{
    private PersonActorExtensionsEntity Entity { get; }
    
    public PersonActorExtension() : this(new TypeMap()) {}
    
    public PersonActorExtension(TypeMap typeMap) : base(typeMap)
        => Entity = TypeMap.Extend<PersonActorExtensionsEntity>();
    
    public PersonActorExtension(ASType existingGraph) : this(existingGraph.TypeMap) {}
    
    [SetsRequiredMembers]
    public PersonActorExtension(TypeMap typeMap, PersonActorExtensionsEntity? entity) : base(typeMap, null)
        => Entity = entity ?? typeMap.AsEntity<PersonActorExtensionsEntity>();

    static PersonActorExtension IASModel<PersonActorExtension>.FromGraph(TypeMap typeMap) => new(typeMap, null);

    public PublicKey? PublicKey
    {
        get => Entity.PublicKey;
        set => Entity.PublicKey = value;
    }
}

public sealed class PersonActorExtensionsEntity : ASEntity<PersonActorExtension, PersonActorExtensionsEntity>
{
    [JsonPropertyName("publicKey")]
    public PublicKey? PublicKey { get; set; }
}
