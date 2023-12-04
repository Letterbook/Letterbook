using ActivityPub.Types;
using ActivityPub.Types.AS;
using Letterbook.Adapter.ActivityPub.Types;
// ReSharper disable InconsistentNaming

namespace Letterbook.Adapter.ActivityPub.Types;

public class ActorExtensions : APActor, IASModel<ActorExtensions, APActorExtensionsEntity, APActor>
{
    private APActorExtensionsEntity Entity { get; }

    public ActorExtensions() => Entity = new APActorExtensionsEntity();
    public ActorExtensions(TypeMap typeMap) : base(typeMap) => Entity = TypeMap.AsEntity<APActorExtensionsEntity>();

    public PublicKey? PublicKey
    {
        get => Entity.PublicKey;
        set => Entity.PublicKey = value!;
    }

    public static ActorExtensions FromGraph(TypeMap typeMap)
    {
        throw new NotImplementedException();
    }
}

public sealed class APActorExtensionsEntity : ASEntity<ActorExtensions, APActorExtensionsEntity>
{
    public PublicKey? PublicKey { get; set; }
}