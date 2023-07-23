namespace Letterbook.Core.Models;

public class Audience : IEquatable<Audience>, IObjectRef
{
    private Audience()
    {
        Id = default!;
    }
    
    public Uri Id { get; set; }
    // LocalId isn't a meaningful concept for Audience, but it's required by IObjectRef
    public string? LocalId { 
        get => Id.ToString(); 
        set => Id = Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out var newId) ? newId : Id; 
    }
    public string Authority => Id.Authority;
    public List<Profile> Members { get; set; } = new();

    public bool Equals(Audience? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id.Equals(other.Id) && Members.Equals(other.Members);
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