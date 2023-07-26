using Letterbook.Core.Extensions;

namespace Letterbook.Core.Models;

public class Account
{
    public string Id { get; set; }
    public string Email { get; set; }
    // TODO(Authentication): https://github.com/Letterbook/Letterbook/issues/35
    // PasswordHash?
    // 2FA stuff?
    public Profile DefaultProfile { get; set; }
    public ICollection<AccountProfile> LinkedProfiles { get; set; } = new HashSet<AccountProfile>();
    
    // In the future, Account might tie in to things like organizations or billing accounts
    
    private Account()
    {
        Id = default!;
        Email = default!;
        DefaultProfile = default!;
    }

    private Account(string email) : this()
    {
        Id = ShortId.NewShortId();
        Email = email;
    }

    // TODO(Account creation): https://github.com/Letterbook/Letterbook/issues/32
    public static Account CreateAccount(string email, Uri profileId)
    {
        var profile = new Profile(profileId)
        {
            LocalId = ShortId.NewShortId()
        };
        var account = new Account(email)
        {
            DefaultProfile = profile
        };
        profile.OwnedBy = account;
        account.LinkedProfiles.Add(new AccountProfile(account, profile, ProfilePermission.All));

        return account;
    }
}