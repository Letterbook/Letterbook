﻿namespace Letterbook.Core.Models;

/// <summary>
/// Audience is the collection or category of profiles that can view some content
/// This usually means something like "everyone" and/or "the followers of the content's creator(s)"
/// Sometimes it's the specifically addressed people, or everyone on the local instance
///
/// Audience targeting is used internally to build feeds and notifications. It's also used externally to imply
/// visibility controls for federated content.
/// </summary>
public class Audience : IEquatable<Audience>, IObjectRef
{
    private static Audience _public = FromUri(new Uri(Constants.ActivityPubPublicCollection), Profile.CreateEmpty(new Uri(Constants.ActivityPubPublicCollection)));
    private Audience()
    {
        Id = default!;
        Source = default!;
    }
    
    public Uri Id { get; set; }
    public Profile Source { get; set; }
    // LocalId isn't a meaningful concept for Audience, but it's required by IObjectRef
    public Guid? LocalId { get; set; }
    public string Authority => Id.Authority;
    public List<Profile> Members { get; set; } = new();

    /// <summary>
    /// No one is actually a member of the public audience. Rather, Letterbook uses it as a signal to build more
    /// specific audiences, as necessary. For instance, not all fedi services will specify the followers audience on
    /// public objects. So Letterbook infers the followers audience in this case.
    /// </summary>
    public static Audience Public => _public;
    public static Audience FromUri(Uri id, Profile source) => new () { Id = id, Source = source};
    public static Audience Followers(Profile creator) => FromUri(creator.Followers, creator);

    public static Audience Subscribers(Profile creator)
    {
        var builder = new UriBuilder(creator.Followers);
        builder.Fragment += "subscribe";
        return FromUri(builder.Uri, creator);
    }
    
    public static Audience Boosts(Profile creator)
    {
        var builder = new UriBuilder(creator.Followers);
        builder.Fragment += "boosts";
        return FromUri(builder.Uri, creator);
    }
    public static Audience FromMention(Profile subject) => FromUri(subject.Id, subject);

    public bool Equals(Audience? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id.ToString().Equals(other.Id.ToString());
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Audience)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id.ToString());
    }

    public static bool operator ==(Audience? left, Audience? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Audience? left, Audience? right)
    {
        return !Equals(left, right);
    }
}