namespace Letterbook.Core.Models;

public class LinkedProfile
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
}