namespace Letterbook.Core.Models;

public class Audience : IEquatable<Audience>
{
    private Audience()
    {
        Id = default!;
    }
    
    public Uri Id { get; set; }
    public List<Profile> Members { get; set; } = new();
    public List<ApObject> Objects { get; set; } = new();

    public bool Equals(Audience? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id.Equals(other.Id) && Members.Equals(other.Members) && Objects.Equals(other.Objects);
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
        return HashCode.Combine(Id);
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