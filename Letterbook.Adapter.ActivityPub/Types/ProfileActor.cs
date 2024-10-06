using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using ActivityPub.Types;
using ActivityPub.Types.AS;

namespace Letterbook.Adapter.ActivityPub.Types;

/// <summary>
/// A unified actor type that implements all the actor extensions Letterbook supports
/// </summary>
/// <remarks>The derived types support for type checking and mapping to specific AP Actor types.</remarks>
public class ProfileActor : APActor, IASModel<ProfileActor, ProfileActorEntity, APActor>
{
	private ProfileActorEntity Entity { get; }

	public ProfileActor() => Entity = TypeMap.Extend<ProfileActor, ProfileActorEntity>();

	public ProfileActor(TypeMap typeMap, bool isExtending = true) : base(typeMap, false)
		=> Entity = TypeMap.ProjectTo<ProfileActor, ProfileActorEntity>(isExtending);

	public ProfileActor(ASType existingGraph) : this(existingGraph.TypeMap)
	{
	}

	[SetsRequiredMembers]
	public ProfileActor(TypeMap typeMap, ProfileActorEntity? entity) : base(typeMap, null)
		=> Entity = entity ?? typeMap.AsEntity<ProfileActor, ProfileActorEntity>();

	static ProfileActor IASModel<ProfileActor>.FromGraph(TypeMap typeMap) => new(typeMap, null);

	public ASLink? SharedInbox
	{
		get => Entity.SharedInbox;
		set => Entity.SharedInbox = value;
	}

	public List<PublicKey> PublicKeys
	{
		get => Entity.PublicKey;
		set => Entity.PublicKey = value;
	}
}

public sealed class ProfileActorEntity : ASEntity<ProfileActor, ProfileActorEntity>
{
	[JsonPropertyName("publicKey")] public List<PublicKey> PublicKey { get; set; } = new();

	[JsonPropertyName("sharedInbox")] public ASLink? SharedInbox { get; set; }
}

public class ProfilePersonActor : ProfileActor, IASModel<ProfilePersonActor, ProfilePersonActorEntity>
{
	private const string ActorType = "Person";
	static string IASModel<ProfilePersonActor>.ASTypeName => ActorType;
	private ProfilePersonActorEntity Entity { get; }
	static ProfilePersonActor IASModel<ProfilePersonActor>.FromGraph(TypeMap typeMap) => new(typeMap, null);

	public ProfilePersonActor() => Entity = TypeMap.Extend<ProfilePersonActor, ProfilePersonActorEntity>();

	public ProfilePersonActor(TypeMap typeMap, bool isExtending = true) : base(typeMap, false)
		=> Entity = TypeMap.ProjectTo<ProfilePersonActor, ProfilePersonActorEntity>(isExtending);

	public ProfilePersonActor(ASType existingGraph) : this(existingGraph.TypeMap)
	{
	}

	[SetsRequiredMembers]
	public ProfilePersonActor(TypeMap typeMap, ProfilePersonActorEntity? entity) : base(typeMap, null)
		=> Entity = entity ?? typeMap.AsEntity<ProfilePersonActor, ProfilePersonActorEntity>();
}

public sealed class ProfilePersonActorEntity : ASEntity<ProfilePersonActor, ProfilePersonActorEntity>
{
}

public class ProfileApplicationActor : ProfileActor, IASModel<ProfileApplicationActor, ProfileApplicationActorEntity>
{
	private const string ModelType = "Application";
	static string IASModel<ProfileApplicationActor>.ASTypeName => ModelType;
	private ProfileApplicationActorEntity Entity { get; }
	static ProfileApplicationActor IASModel<ProfileApplicationActor>.FromGraph(TypeMap typeMap) => new(typeMap, null);

	public ProfileApplicationActor() => Entity = TypeMap.Extend<ProfileApplicationActor, ProfileApplicationActorEntity>();

	public ProfileApplicationActor(TypeMap typeMap, bool isExtending = true) : base(typeMap, false)
		=> Entity = TypeMap.ProjectTo<ProfileApplicationActor, ProfileApplicationActorEntity>(isExtending);

	public ProfileApplicationActor(ASType existingGraph) : this(existingGraph.TypeMap)
	{
	}

	[SetsRequiredMembers]
	public ProfileApplicationActor(TypeMap typeMap, ProfileApplicationActorEntity? entity) : base(typeMap, null)
		=> Entity = entity ?? typeMap.AsEntity<ProfileApplicationActor, ProfileApplicationActorEntity>();
}

public sealed class ProfileApplicationActorEntity : ASEntity<ProfileApplicationActor, ProfileApplicationActorEntity>
{
}

public class ProfileServiceActor : ProfileActor, IASModel<ProfileServiceActor, ProfileServiceActorEntity>
{
	private const string ModelType = "Service";
	static string IASModel<ProfileServiceActor>.ASTypeName => ModelType;
	private ProfileServiceActorEntity Entity { get; }
	static ProfileServiceActor IASModel<ProfileServiceActor>.FromGraph(TypeMap typeMap) => new(typeMap, null);

	public ProfileServiceActor() => Entity = TypeMap.Extend<ProfileServiceActor, ProfileServiceActorEntity>();

	public ProfileServiceActor(TypeMap typeMap, bool isExtending = true) : base(typeMap, false)
		=> Entity = TypeMap.ProjectTo<ProfileServiceActor, ProfileServiceActorEntity>(isExtending);

	public ProfileServiceActor(ASType existingGraph) : this(existingGraph.TypeMap)
	{
	}

	[SetsRequiredMembers]
	public ProfileServiceActor(TypeMap typeMap, ProfileServiceActorEntity? entity) : base(typeMap, null)
		=> Entity = entity ?? typeMap.AsEntity<ProfileServiceActor, ProfileServiceActorEntity>();
}

public sealed class ProfileServiceActorEntity : ASEntity<ProfileServiceActor, ProfileServiceActorEntity>
{
}

public class ProfileGroupActor : ProfileActor, IASModel<ProfileGroupActor, ProfileGroupActorEntity>
{
	private const string ModelType = "Group";
	static string IASModel<ProfileGroupActor>.ASTypeName => ModelType;
	private ProfileGroupActorEntity Entity { get; }
	static ProfileGroupActor IASModel<ProfileGroupActor>.FromGraph(TypeMap typeMap) => new(typeMap, null);

	public ProfileGroupActor() => Entity = TypeMap.Extend<ProfileGroupActor, ProfileGroupActorEntity>();

	public ProfileGroupActor(TypeMap typeMap, bool isExtending = true) : base(typeMap, false)
		=> Entity = TypeMap.ProjectTo<ProfileGroupActor, ProfileGroupActorEntity>(isExtending);

	public ProfileGroupActor(ASType existingGraph) : this(existingGraph.TypeMap)
	{
	}

	[SetsRequiredMembers]
	public ProfileGroupActor(TypeMap typeMap, ProfileGroupActorEntity? entity) : base(typeMap, null)
		=> Entity = entity ?? typeMap.AsEntity<ProfileGroupActor, ProfileGroupActorEntity>();
}

public sealed class ProfileGroupActorEntity : ASEntity<ProfileGroupActor, ProfileGroupActorEntity>
{
}

public class ProfileOrganizationActor : ProfileActor, IASModel<ProfileOrganizationActor, ProfileOrganizationActorEntity>
{
	private const string ModelType = "Organization";
	static string IASModel<ProfileOrganizationActor>.ASTypeName => ModelType;
	private ProfileOrganizationActorEntity Entity { get; }
	static ProfileOrganizationActor IASModel<ProfileOrganizationActor>.FromGraph(TypeMap typeMap) => new(typeMap, null);

	public ProfileOrganizationActor() => Entity = TypeMap.Extend<ProfileOrganizationActor, ProfileOrganizationActorEntity>();

	public ProfileOrganizationActor(TypeMap typeMap, bool isExtending = true) : base(typeMap, false)
		=> Entity = TypeMap.ProjectTo<ProfileOrganizationActor, ProfileOrganizationActorEntity>(isExtending);

	public ProfileOrganizationActor(ASType existingGraph) : this(existingGraph.TypeMap)
	{
	}

	[SetsRequiredMembers]
	public ProfileOrganizationActor(TypeMap typeMap, ProfileOrganizationActorEntity? entity) : base(typeMap, null)
		=> Entity = entity ?? typeMap.AsEntity<ProfileOrganizationActor, ProfileOrganizationActorEntity>();
}

public sealed class ProfileOrganizationActorEntity : ASEntity<ProfileOrganizationActor, ProfileOrganizationActorEntity>
{
}