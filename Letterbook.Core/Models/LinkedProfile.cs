namespace Letterbook.Core.Models;

public class LinkedProfile : IEquatable<LinkedProfile>
{
    public Account Account { get; set; }
    public Profile Profile { get; set; }
    public ProfilePermission Permission { get; set; }

    private LinkedProfile()
    {
        Account = default!;
        Profile = default!;
        Permission = default!;
    }

    public LinkedProfile(Account account, Profile profile, ProfilePermission permission)
    {
        Account = account;
        Profile = profile;
        Permission = permission;
    }

    public bool Equals(LinkedProfile? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Account.Equals(other.Account) && Profile.Equals(other.Profile);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((LinkedProfile)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Account, Profile);
    }

    public static bool operator ==(LinkedProfile? left, LinkedProfile? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(LinkedProfile? left, LinkedProfile? right)
    {
        return !Equals(left, right);
    }
}