using Microsoft.AspNetCore.Identity;

namespace Letterbook.Core.Models;

public class AccountIdentity : IdentityUser<Guid>
{
    public Account Account { get; set; }

    private AccountIdentity()
    {
        Account = default!;
    }

    public AccountIdentity(Account account, string email, string? username)
    {
        Account = account;
        base.Email = email;
        base.EmailConfirmed = false;
        base.UserName = username;
    }
}