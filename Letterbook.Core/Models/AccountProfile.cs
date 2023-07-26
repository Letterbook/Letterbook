namespace Letterbook.Core.Models;

public class AccountProfile
{
    public Account Account { get; set; }
    public Profile Profile { get; set; }
    public ProfilePermission Permission { get; set; }

    private AccountProfile()
    {
        Account = default!;
        Profile = default!;
        Permission = default!;
    }

    public AccountProfile(Account account, Profile profile, ProfilePermission permission)
    {
        Account = account;
        Profile = profile;
        Permission = permission;
    }
}