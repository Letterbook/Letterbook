namespace Letterbook.Core.Models;

// A Mention is when some kind of object (usually a Note, sometimes an Image, theoretically others) is addressed to 
// another individual Actor at any level of visibility.
public class Mention : IEquatable<Mention>
{
    public IObjectRef Source { get; set; }
    public Actor Subject { get; set; }
    public MentionVisibility Visibility { get; set; }

    public bool Equals(Mention? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Source.Equals(other.Source) && Subject.Equals(other.Subject) && Visibility == other.Visibility;
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
        return HashCode.Combine(Source.Id, Subject);
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