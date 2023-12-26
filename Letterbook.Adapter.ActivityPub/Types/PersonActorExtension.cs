using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using ActivityPub.Types;
using ActivityPub.Types.AS;
using ActivityPub.Types.AS.Extended.Actor;

namespace Letterbook.Adapter.ActivityPub.Types;

public class PersonActorExtension : PersonActor, IASModel<PersonActorExtension, PersonActorExtensionEntity, PersonActor>
{
    private PersonActorExtensionEntity Entity { get; }
    
    public PersonActorExtension() => Entity = TypeMap.Extend<PersonActorExtension, PersonActorExtensionEntity>();
    
    public PersonActorExtension(TypeMap typeMap, bool isExtending = true) : base(typeMap, false)
        => Entity = TypeMap.ProjectTo<PersonActorExtension, PersonActorExtensionEntity>(isExtending);
    
    public PersonActorExtension(ASType existingGraph) : this(existingGraph.TypeMap) {}
    
    [SetsRequiredMembers]
    public PersonActorExtension(TypeMap typeMap, PersonActorExtensionEntity? entity) : base(typeMap, null)
        => Entity = entity ?? typeMap.AsEntity<PersonActorExtension, PersonActorExtensionEntity>();

    static PersonActorExtension IASModel<PersonActorExtension>.FromGraph(TypeMap typeMap) => new(typeMap, null);

    public PublicKey? PublicKey
    {
        get => Entity.PublicKey;
        set => Entity.PublicKey = value;
    }
}

public sealed class PersonActorExtensionEntity : ASEntity<PersonActorExtension, PersonActorExtensionEntity>
{
    [JsonPropertyName("publicKey")]
    public PublicKey? PublicKey { get; set; }
}
