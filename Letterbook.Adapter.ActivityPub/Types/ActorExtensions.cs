using System.Diagnostics.CodeAnalysis;
using ActivityPub.Types;
using ActivityPub.Types.AS;
// using Letterbook.Adapter.ActivityPub.Types;
// ReSharper disable InconsistentNaming

namespace Letterbook.Adapter.ActivityPub.Types;

public class ActorExtensions : APActor, IASModel<ActorExtensions, APActorExtensionsEntity, APActor>
{
    private APActorExtensionsEntity Entity { get; }
    
    public ActorExtensions() => Entity = TypeMap.Extend<APActorExtensionsEntity>();
    
    public ActorExtensions(TypeMap typeMap, bool isExtending = true) : base(typeMap, false)
        => Entity = TypeMap.ProjectTo<APActorExtensionsEntity>(isExtending);
    
    public ActorExtensions(ASType existingGraph) : this(existingGraph.TypeMap) {}
    
    [SetsRequiredMembers]
    public ActorExtensions(TypeMap typeMap, APActorExtensionsEntity? entity) : base(typeMap, null)
        => Entity = entity ?? typeMap.AsEntity<APActorExtensionsEntity>();

    static ActorExtensions IASModel<ActorExtensions>.FromGraph(TypeMap typeMap) => new(typeMap, null);

    public PublicKey? PublicKey
    {
        get => Entity.PublicKey;
        set => Entity.PublicKey = value;
    }
}

public sealed class APActorExtensionsEntity : ASEntity<ActorExtensions, APActorExtensionsEntity>
{
    public PublicKey? PublicKey { get; set; }
}