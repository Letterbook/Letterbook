using Medo;

namespace Letterbook.Core.Models;

/// <summary>
/// A Mention is when a Post is addressed to another individual Profile at any level of visibility.
/// </summary>
public class Mention : IEquatable<Mention>
{
    private static readonly Mention PublicSpecialMention = new()
    {
        Id = Guid.Empty,
        Subject = Profile.CreateEmpty(new Uri(Constants.ActivityPubPublicCollection)),
        Visibility = MentionVisibility.To
    };

    private static readonly Mention UnlistedSpecialMention = new()
    {
        Id = Guid.Empty,
        Subject = Profile.CreateEmpty(new Uri(Constants.ActivityPubPublicCollection)),
        Visibility = MentionVisibility.Cc
    };

    private Uuid7 _id;

    public Guid Id
    {
        get => _id.ToGuid();
        set => _id = Uuid7.FromGuid(value);
    }

    public Profile Subject { get; set; }
    public MentionVisibility Visibility { get; set; }

    public static Mention Public => PublicSpecialMention;
    public static Mention Unlisted => UnlistedSpecialMention;
    public static Mention To(Profile subject) => Create(subject, MentionVisibility.To);
    public static Mention Bto(Profile subject) => Create(subject, MentionVisibility.Bto);
    public static Mention Cc(Profile subject) => Create(subject, MentionVisibility.Cc);
    public static Mention Bcc(Profile subject) => Create(subject, MentionVisibility.Bcc);

    private Mention()
    {
        Subject = default!;
    }

    public Mention(Profile subject, MentionVisibility visibility)
    {
        _id = Uuid7.NewUuid7();
        Subject = subject;
        Visibility = visibility;
    }

    public Uuid7 GetId() => _id;
    public string GetId25() => _id.ToId25String();

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
            _id = Uuid7.NewUuid7(),
            Subject = subject,
            Visibility = visibility
        };
    }
}