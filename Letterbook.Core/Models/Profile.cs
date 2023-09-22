using System.Collections;
using Letterbook.Core.Extensions;
using Letterbook.Core.Values;

namespace Letterbook.Core.Models;

/// <summary>
/// A Profile is the externally visible representation of an account on the network. In ActivityPub terms, it should map
/// 1:1 with Actors.
/// Local profiles are managed by one or more Accounts, which are the representation of a user internally to the system.
/// Remote profiles have no associated Accounts, and can only be created or modified by federated changes to the remote
/// Actor.
/// </summary>
public class Profile : IObjectRef, IEquatable<Profile>
{
    private Profile()
    {
        Id = default!;
        Inbox = default!;
        Outbox = default!;
        Type = default;
        Handle = default!;
        DisplayName = default!;
        FollowersCollection = default!;
        CustomFields = default!;
        Description = default!;
    }
    
    // Constructor for local profiles
    private Profile(Uri baseUri, Guid id) : this()
    {
        Id = new Uri(baseUri, $"/actor/{id.ToShortId()}");
        Handle = string.Empty;
        DisplayName = string.Empty;
        Description = string.Empty;
        CustomFields = Array.Empty<CustomField>();
        
        var builder = new UriBuilder(Id);
        var basePath = builder.Path;
        LocalId = id;

        builder.Path = basePath + "/inbox";
        Inbox = builder.Uri;

        builder.Path = basePath + "/outbox";
        Outbox = builder.Uri;

        builder.Path = "/actor/shared_inbox";
        SharedInbox = builder.Uri;
        FollowersCollection = ObjectCollection<FollowerRelation>.Followers(Id);
    }

    public Uri Id { get; set; }
    public Uri Inbox { get; set; }
    public Uri Outbox { get; set; }
    public Uri? SharedInbox { get; set; }
    public Guid? LocalId { get; set; }
    public string Authority => Id.Authority;
    public string Handle { get; set; }
    public string DisplayName { get; set; }
    public string Description { get; set; }
    public CustomField[] CustomFields { get; set; }
    public DateTime Updated { get; set; } = DateTime.UtcNow;

    // Local profiles should all have an owner, but remote ones do not.
    // Could remote profiles be claimed through an account transfer?
    public Account? OwnedBy { get; set; }
    public ActivityActorType Type { get; set; }
    public ICollection<Audience> Audiences { get; set; } = new HashSet<Audience>();
    public ICollection<LinkedProfile> RelatedAccounts { get; set; } = new HashSet<LinkedProfile>();
    public ObjectCollection<FollowerRelation> FollowersCollection { get; set; }
    public ICollection<FollowerRelation> Following { get; set; } = new HashSet<FollowerRelation>();

    public Profile ShallowClone() => (Profile)MemberwiseClone();

    public FollowerRelation AddFollower(Profile follower, FollowState state)
    {
        var relation = new FollowerRelation(this, follower, state);
        FollowersCollection.Add(relation);
        return relation;
    }

    public FollowerRelation AddFollowing(Profile following, FollowState state)
    {
        var relation = new FollowerRelation(following, this, state);
        Following.Add(relation);
        return relation;
    }
    
    // Eventually: CreateGroup, CreateBot, Mayyyyyybe CreateService?
    // The only use case I'm imagining for a service is to represent the server itself
    public static Profile CreateIndividual(Uri baseUri, string handle)
    {
        var localId = Guid.NewGuid();
        var profile = new Profile(baseUri, localId)
        {
            Type = ActivityActorType.Person,
            Handle = $"@{handle}@{baseUri.Authority}",
            DisplayName = handle,
        };
        profile.Audiences.Add(Audience.FromMention(profile));
        return profile;
    }

    // Really only useful for doing equality comparisons, but that's a thing we do sometimes.
    public static Profile CreateEmpty(Uri id)
    {
        return new Profile()
        {
            Id = id
        };
    }

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