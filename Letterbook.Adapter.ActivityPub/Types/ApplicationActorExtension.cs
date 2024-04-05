using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using ActivityPub.Types;
using ActivityPub.Types.AS;
using ActivityPub.Types.AS.Extended.Actor;

namespace Letterbook.Adapter.ActivityPub.Types;

public class ApplicationActorExtension : ApplicationActor, IASModel<ApplicationActorExtension, ApplicationActorExtensionEntity, ApplicationActor>
{
	private ApplicationActorExtensionEntity Entity { get; }

	public ApplicationActorExtension() => Entity = TypeMap.Extend<ApplicationActorExtension, ApplicationActorExtensionEntity>();

	public ApplicationActorExtension(TypeMap typeMap, bool isExtending = true) : base(typeMap, false)
		=> Entity = TypeMap.ProjectTo<ApplicationActorExtension, ApplicationActorExtensionEntity>(isExtending);

	public ApplicationActorExtension(ASType existingGraph) : this(existingGraph.TypeMap) { }

	[SetsRequiredMembers]
	public ApplicationActorExtension(TypeMap typeMap, ApplicationActorExtensionEntity? entity) : base(typeMap, null)
		=> Entity = entity ?? typeMap.AsEntity<ApplicationActorExtension, ApplicationActorExtensionEntity>();

	static ApplicationActorExtension IASModel<ApplicationActorExtension>.FromGraph(TypeMap typeMap) => new(typeMap, null);

	public PublicKey? PublicKey
	{
		get => Entity.PublicKey;
		set => Entity.PublicKey = value;
	}
}

public sealed class ApplicationActorExtensionEntity : ASEntity<ApplicationActorExtension, ApplicationActorExtensionEntity>
{
	[JsonPropertyName("publicKey")]
	public PublicKey? PublicKey { get; set; }
}
