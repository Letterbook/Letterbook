using System.Collections.Concurrent;
using System.Security.Claims;
using Letterbook.Core.Extensions;
using Letterbook.Core.Values;
using Letterbook.Generators;
using Medo;

namespace Letterbook.Core.Models;

public partial record struct ProfileId(Uuid7 Id) : ITypedId<Uuid7>;

/// <summary>
/// A Profile is the externally visible representation of an account on the network. In ActivityPub terms, it should map
/// 1:1 with Actors.
/// Local profiles are managed by one or more Accounts, which are the representation of a user internally to the system.
/// Remote profiles have no associated Accounts, and can only be created or modified by federated changes to the remote
/// Actor.
/// </summary>
public class Profile : IFederatedActor, IEquatable<Profile>
{
	private Profile()
	{
		FediId = default!;
		Authority = default!;
		Inbox = default!;
		Outbox = default!;
		Followers = default!;
		Following = default!;
		Type = default;
		Handle = default!;
		DisplayName = default!;
		Description = default!;
	}

	private Profile(Uri baseUri) : this(new(Uuid7.NewUuid7()), baseUri) { }

	// Constructor for local profiles
	private Profile(ProfileId id, Uri baseUri) : this()
	{
		Id = id;
		FediId = new Uri(baseUri, $"/actor/{Id}");
		Authority = FediId.GetAuthority();
		Handle = string.Empty;
		DisplayName = string.Empty;
		Description = string.Empty;

		var builder = new UriBuilder(FediId);
		var basePath = builder.Path;

		builder.Path = basePath + "/inbox";
		Inbox = builder.Uri;

		builder.Path = basePath + "/outbox";
		Outbox = builder.Uri;

		builder.Path = basePath + "/followers";
		Followers = builder.Uri;

		builder.Path = basePath + "/following";
		Following = builder.Uri;

		builder.Path = "/actor/shared_inbox";
		SharedInbox = builder.Uri;

		builder.Path = basePath;
		builder.Fragment = "key-0";
		Keys.Add(SigningKey.Rsa(0, builder.Uri));
		builder.Fragment = "key-1";
		Keys.Add(SigningKey.EcDsa(1, builder.Uri));
	}

	private static readonly ConcurrentDictionary<ProfileId, Profile> SystemProfiles = new();
	public static readonly ProfileId SystemModeratorsId = new Uuid7(new byte[] { 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
	public static readonly ProfileId SystemInstanceId = new Uuid7(new byte[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });

	/// <summary>
	/// A reserved system profile to be used by moderators for federating moderation reports and related correspondence
	/// </summary>
	/// <remarks>
	/// You SHOULD NOT USE the Keys or other collections on this object. Those must be queried from the data adapter. This static instance
	/// can be used to seed the database, and quickly look up the profile's various identifiers.
	/// </remarks>
	/// <param name="opts"></param>
	/// <returns></returns>
	public static Profile GetOrAddModeratorsProfile(CoreOptions opts) => SystemProfiles.GetOrAdd(SystemModeratorsId, id => new Profile(id, opts.BaseUri())
	{
		Type = ActivityActorType.Service,
		Handle = "Moderators",
		DisplayName = "Moderators",
		Description = $"The special reserved profile for the {opts.DomainName} moderators",
		OwnedBy = new Account() { UserName = "System Moderator account" }
	});

	public static Profile AddModeratorsProfile(Profile moderators) => SystemProfiles[SystemModeratorsId] = moderators;
	public static Profile? GetModeratorsProfile() => SystemProfiles.GetValueOrDefault(SystemModeratorsId);

	/// <summary>
	/// A reserved system profile to be used to represent the system itself, when no other profile is better suited
	/// </summary>
	/// <remarks>
	/// You SHOULD NOT USE the Keys or other collections on this object. Those must be queried from the data adapter. This static instance
	/// can be used to seed the database, and quickly look up the profile's various identifiers.
	/// </remarks>
	/// <param name="opts"></param>
	/// <returns></returns>
	public static Profile GetOrAddInstanceProfile(CoreOptions opts) => SystemProfiles.GetOrAdd(SystemInstanceId, id => new Profile(id, opts.BaseUri())
	{
		Type = ActivityActorType.Service,
		Handle = opts.DomainName,
		DisplayName = "Letterbook",
		Description = $"The special system profile for the {opts.DomainName} server instance"
	});

	public static Profile AddInstanceProfile(Profile instance) => SystemProfiles[SystemInstanceId] = instance;
	public static Profile? GetInstanceProfile() => SystemProfiles.GetValueOrDefault(SystemInstanceId);

	public ProfileId Id { get; set; }

	public Uri FediId { get; set; }
	public Uri Inbox { get; set; }
	public Uri Outbox { get; set; }
	public Uri? SharedInbox { get; set; }
	public Uri Followers { get; set; }
	public Uri Following { get; set; }
	public string Authority { get; private set; }
	public string Handle { get; set; }
	public string DisplayName { get; set; }
	public string Description { get; set; }
	public CustomField[] CustomFields { get; set; } = [];
	public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;
	public DateTime Updated { get; set; } = DateTime.UtcNow;
	public Account? OwnedBy { get; set; }
	public ICollection<ProfileClaims> Accessors { get; set; } = new HashSet<ProfileClaims>();
	public ActivityActorType Type { get; set; }
	public ICollection<Audience> Headlining { get; set; } = new HashSet<Audience>();
	public ICollection<Audience> Audiences { get; set; } = new HashSet<Audience>();
	public IList<FollowerRelation> FollowersCollection { get; set; } = new List<FollowerRelation>();
	public IList<FollowerRelation> FollowingCollection { get; set; } = new List<FollowerRelation>();
	public IList<SigningKey> Keys { get; set; } = new List<SigningKey>();
	/// This Profile was the subject of these Reports
	public ICollection<ModerationReport> ReportSubject = new HashSet<ModerationReport>();
	/// This Profile submitted these Reports
	public ICollection<ModerationReport> Reports = new HashSet<ModerationReport>();
	public IDictionary<Restrictions, DateTimeOffset> Restrictions { get; set; } = new Dictionary<Restrictions, DateTimeOffset>();

	public IEnumerable<Claim> RestrictionClaims() => Restrictions
		.Where(r => r.Value >= DateTimeOffset.UtcNow)
		.Select(pair => new Claim(pair.Key.ToString(), "true"));
	public Uuid7 GetId() => Id.Id;
	public string GetId25() => Id.ToString();
	public Profile ShallowClone() => (Profile)MemberwiseClone();

	public Profile ShallowCopy(Profile? copyFrom)
	{
		if (copyFrom is null) return this;
		if (!Equals(copyFrom)) return this;

		Inbox = copyFrom.Inbox ?? Inbox;
		Outbox = copyFrom.Outbox ?? Outbox;
		SharedInbox = copyFrom.SharedInbox ?? SharedInbox;
		Type = copyFrom.Type == ActivityActorType.Unknown ? Type : copyFrom.Type;
		Handle = string.IsNullOrEmpty(copyFrom.Handle) ? Handle : copyFrom.Handle;
		DisplayName = string.IsNullOrEmpty(copyFrom.DisplayName) ? DisplayName : copyFrom.DisplayName;
		CustomFields = (copyFrom.CustomFields is not null && copyFrom.CustomFields.Length != 0)
			? copyFrom.CustomFields
			: CustomFields;
		Description = copyFrom.Description ?? Description;

		return this;
	}

	public FollowerRelation AddFollower(Profile follower, FollowState state)
	{
		var relation = new FollowerRelation(follower, this, state);
		FollowersCollection.Add(relation);
		follower.FollowingCollection.Add(relation);
		return relation;
	}

	public int RemoveFollower(Profile follower)
	{
		var matches = FollowersCollection.Where(relation => relation.Follower == follower).ToList();
		foreach (var match in matches)
		{
			FollowersCollection.Remove(match);
		}

		return matches.Count();
	}

	public FollowerRelation Follow(Profile following, FollowState state)
	{
		var relation = new FollowerRelation(this, following, state);
		FollowingCollection.Add(relation);
		following.FollowersCollection.Add(relation);

		if (state != FollowState.Accepted) return relation;
		var joining = new HashSet<Audience>();

		joining.Add(Audience.Followers(following));
		joining.Add(Audience.Boosts(following));
		foreach (var audience in joining.ReplaceFrom(following.Headlining))
		{
			Audiences.Add(audience);
		}

		return relation;
	}

	public int Unfollow(Profile following)
	{
		var count = 0;
		var targets = FollowingCollection.Where(relation => relation.Follows == following).ToList();
		foreach (var target in targets)
		{
			FollowingCollection.Remove(target);
			count++;
		}

		return count;
	}

	public int LeaveAudience(Profile following)
	{
		if (Audiences is HashSet<Audience> memberships)
		{
			return memberships.RemoveWhere(m => m.Source == following);
		}

		var count = 0;
		var targets = Audiences.Where(m => m.Source == following);
		foreach (var target in targets)
		{
			Audiences.Remove(target);
			count++;
		}

		return count;
	}

	public int RemoveFromAudience(Profile member)
	{
		return Headlining.Sum(audience => audience.Members.Remove(member) ? 1 : 0);
	}

	public FollowerRelation Block(Profile target)
	{
		var follower = FollowersCollection.FirstOrDefault(relation => relation.Follower == target) ?? target.RelationWith(this);
		follower.State = FollowState.Blocked;
		FollowersCollection.Add(follower);

		var following = FollowingCollection.FirstOrDefault(relation => relation.Follows == target) ?? RelationWith(target);
		if(following.State != FollowState.Blocked) following.State = FollowState.None;
		FollowingCollection.Add(following);

		RemoveFromAudience(target);
		LeaveAudience(target);

		return follower;
	}

	public FollowerRelation Unblock(Profile target)
	{
		var follower = FollowersCollection.FirstOrDefault(relation => relation.Follower == target) ?? target.RelationWith(this);
		follower.State = FollowState.None;

		return follower;
	}

	public bool HasBlocked(Profile target)
	{
		var relation = FollowersCollection.FirstOrDefault(relation => relation.Follower == target,
			new FollowerRelation(target, this, FollowState.None));
		return relation.State == FollowState.Blocked;
	}

	// Eventually: CreateGroup, CreateBot, Mayyyyyybe CreateService?
	// The only use case I'm imagining for a service is to represent the server itself
	public static Profile CreateIndividual(Uri baseUri, string handle) => Create(baseUri, handle, ActivityActorType.Person);
	public static Profile CreateSystemActor(Uri baseUri, string handle) => Create(baseUri, handle, ActivityActorType.Application);

	private static Profile Create(Uri baseUri, string handle, ActivityActorType type)
	{
		var profile = new Profile(baseUri)
		{
			Type = type,
			Handle = handle,
			DisplayName = handle,
		};
		profile.Audiences.Add(Audience.FromMention(profile));
		profile.Headlining.Add(Audience.Followers(profile));
		return profile;
	}

	// Really only useful for doing equality comparisons, but that's a thing we do sometimes.
	// Don't persist this anywhere.
	public static Profile CreateEmpty(Uri id)
	{
		return new Profile()
		{
			Id = new(Uuid7.NewUuid7()),
			FediId = id,
			Authority = id.GetAuthority(),
			Handle = "",
			DisplayName = "",
			Description = "",
			// TODO: this is a hack, not all profiles will have AP collections
			// Need to come up with non-null zero values for these
			Inbox = id,
			Outbox = id,
			Followers = id,
			Following = id
		};
	}

	public static Profile CreateEmpty(ProfileId id) => new() { Id = id };
	public static Profile CreateEmpty(ProfileId id, Uri fediId) => new() { Id = id, FediId = fediId };

	/// private

	private FollowerRelation RelationWith(Profile target) => new(this, target, FollowState.None);

	public bool Equals(Profile? other)
	{
		if (ReferenceEquals(null, other)) return false;
		if (ReferenceEquals(this, other)) return true;
		return Id.Equals(other.Id);
	}

	public override bool Equals(object? obj)
	{
		if (ReferenceEquals(null, obj)) return false;
		if (ReferenceEquals(this, obj)) return true;
		if (obj.GetType() != this.GetType()) return false;
		return Equals((Profile)obj);
	}

	public override int GetHashCode()
	{
		return Id.GetHashCode();
	}

	public static bool operator ==(Profile? left, Profile? right)
	{
		return Equals(left, right);
	}

	public static bool operator !=(Profile? left, Profile? right)
	{
		return !Equals(left, right);
	}
}