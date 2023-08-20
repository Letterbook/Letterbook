using Letterbook.ActivityPub;

namespace Letterbook.Core.Models;

/// <summary>
/// A Mention is when some kind of object (usually a Note, sometimes an Image, theoretically others) is addressed to 
/// another individual Actor at any level of visibility.
/// </summary>
public class Mention : IEquatable<Mention>
{
    private static Mention _publicSpecialMention = new()
    {
        Id = Guid.Empty,
        Subject = new Profile(new CompactIri("as", "public")),
        Visibility = MentionVisibility.To
    };

    private static Mention _unlistedSpecialMention = new()
    {
        Id = Guid.Empty,
        Subject = new Profile(new CompactIri("as", "public")),
        Visibility = MentionVisibility.Cc
    };

    public Guid Id { get; set; }
    public Profile Subject { get; set; }
    public MentionVisibility Visibility { get; set; }

    public static Mention Public => _publicSpecialMention;
    public static Mention Unlisted => _unlistedSpecialMention;
    public static Mention To(Profile subject) => Create(subject, MentionVisibility.To);
    public static Mention Bto(Profile subject) => Create(subject, MentionVisibility.Bto);
    public static Mention Cc(Profile subject) => Create(subject, MentionVisibility.Cc);
    public static Mention Bcc(Profile subject) => Create(subject, MentionVisibility.Bcc);

    /// <summary>
    /// This should be used for comparisons only, in visibility calculations. It will not make any sense to persist it
    /// </summary>
    /// <param name="subject"></param>
    /// <returns></returns>
    public static Mention Followers(Profile subject) => new()
    {
        Id = Guid.NewGuid(),
        Subject = new Profile(subject.FollowersCollection.Id),
        Visibility = MentionVisibility.To
    };

    public bool Equals(Mention? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Subject.Equals(other.Subject);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Mention)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Subject, Visibility);
    }

    public static bool operator ==(Mention? left, Mention? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Mention? left, Mention? right)
    {
        return !Equals(left, right);
    }

    private static Mention Create(Profile subject, MentionVisibility visibility)
    {
        return new Mention()
        {
            Id = Guid.NewGuid(),
            Subject = subject,
            Visibility = visibility
        };
    }
}