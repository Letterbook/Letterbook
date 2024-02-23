using Medo;

namespace Letterbook.Core.Models;

public class ProfileAccess : IEquatable<ProfileAccess>
{
    private Uuid7 _id;

    public Guid Id
    {
        get => _id.ToGuid();
        set => _id = Uuid7.FromGuid(value);
    }

    public Profile Profile { get; set; }
    public Account Account { get; set; }
    public ProfilePermission Permission { get; set; }

    private ProfileAccess()
    {
        Id = Uuid7.NewUuid7();
        Profile = default!;
        Account = default!;
        Permission = default!;
    }

    public ProfileAccess(Account account, Profile profile, ProfilePermission permission)
    {
        Id = Uuid7.NewUuid7();
        Account = account;
        Profile = profile;
        Permission = permission;
    }
    
    public Uuid7 GetId() => _id;
    public string GetId25() => _id.ToId25String();

    public bool Equals(ProfileAccess? other)
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
        return Equals((ProfileAccess)obj);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static bool operator ==(ProfileAccess? left, ProfileAccess? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(ProfileAccess? left, ProfileAccess? right)
    {
        return !Equals(left, right);
    }
}