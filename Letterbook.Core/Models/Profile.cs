using System.Collections;
using Letterbook.Core.Extensions;
using Letterbook.Core.Values;
using Medo;

namespace Letterbook.Core.Models;

/// <summary>
/// A Profile is the externally visible representation of an account on the network. In ActivityPub terms, it should map
/// 1:1 with Actors.
/// Local profiles are managed by one or more Accounts, which are the representation of a user internally to the system.
/// Remote profiles have no associated Accounts, and can only be created or modified by federated changes to the remote
/// Actor.
/// </summary>
public class Profile : IFederated, IEquatable<Profile>
{
	private Uuid7 _id;

	private Profile()
	{
		FediId = default!;
		Inbox = default!;
		Outbox = default!;
		Followers = default!;
		Following = default!;
		Type = default;
		Handle = default!;
		DisplayName = default!;
		CustomFields = default!;
		Description = default!;
	}

	// Constructor for local profiles
	private Profile(Uri baseUri) : this()
	{
		_id = Uuid7.NewUuid7();
		FediId = new Uri(baseUri, $"/actor/{_id.ToId25String()}");
		Handle = string.Empty;
		DisplayName = string.Empty;
		Description = string.Empty;
		CustomFields = Array.Empty<CustomField>();

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
		builder.Fragment = "public_keys/0";
		Keys.Add(SigningKey.Rsa(0, builder.Uri));
		builder.Fragment = "public_keys/1";
		Keys.Add(SigningKey.EcDsa(1, builder.Uri));
	}

	public Guid Id
	{
		get => _id.ToGuid();
		set => _id = Uuid7.FromGuid(value);
	}

	public Uri FediId { get; set; }
	public Uri Inbox { get; set; }
	public Uri Outbox { get; set; }
	public Uri? SharedInbox { get; set; }
	public Uri Followers { get; set; }
	public Uri Following { get; set; }
	public string Authority => FediId.Authority;
	public string Handle { get; set; }
	public string DisplayName { get; set; }
	public string Description { get; set; }
	public CustomField[] CustomFields { get; set; }
	public DateTime Updated { get; set; } = DateTime.UtcNow;
	public Account? OwnedBy { get; set; }
	public ICollection<ProfileAccess> Accessors { get; set; } = new HashSet<ProfileAccess>();
	public ActivityActorType Type { get; set; }
	public ICollection<Audience> Audiences { get; set; } = new HashSet<Audience>();
	public IList<FollowerRelation> FollowersCollection { get; set; } = new List<FollowerRelation>();
	public IList<FollowerRelation> FollowingCollection { get; set; } = new List<FollowerRelation>();
	public IList<SigningKey> Keys { get; set; } = new List<SigningKey>();

	public Uuid7 GetId() => _id;
	public string GetId25() => _id.ToId25String();
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

	// Eventually: CreateGroup, CreateBot, Mayyyyyybe CreateService?
	// The only use case I'm imagining for a service is to represent the server itself
	public static Profile CreateIndividual(Uri baseUri, string handle)
	{
		var profile = new Profile(baseUri)
		{
			Type = ActivityActorType.Person,
			Handle = handle,
			DisplayName = handle,
		};
		profile.Audiences.Add(Audience.FromMention(profile));
		return profile;
	}

	// Really only useful for doing equality comparisons, but that's a thing we do sometimes.
	// Don't persist this anywhere.
	public static Profile CreateEmpty(Uri id)
	{
		return new Profile()
		{
			_id = Uuid7.NewUuid7(),
			FediId = id
		};
	}

	public static Profile CreateEmpty(Uuid7 id) => new() { _id = id };
	public static Profile CreateEmpty(Uuid7 id, Uri fediId) => new() { _id = id, FediId = fediId };

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