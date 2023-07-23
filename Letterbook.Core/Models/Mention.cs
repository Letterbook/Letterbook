namespace Letterbook.Core.Models;

/// <summary>
/// A Mention is when some kind of object (usually a Note, sometimes an Image, theoretically others) is addressed to 
/// another individual Actor at any level of visibility.
///
/// Note: Mentions are frequently collected into HashSets. This is really convenient, because among other things it will
/// deduplicate them essentially for free. Be sure to add them in order from most to least visible, so that publicly
/// visible mentions (To and Cc) don't get hidden by redundant private mentions (Bto and Bcc). 
/// </summary>
public class Mention : IEquatable<Mention>
{
    public Guid Id { get; set; }
    public Profile Subject { get; set; }
    public MentionVisibility Visibility { get; set; }

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
        return HashCode.Combine(Subject);
    }

    public static bool operator ==(Mention? left, Mention? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Mention? left, Mention? right)
    {
        return !Equals(left, right);
    }
}