using Medo;
using Microsoft.AspNetCore.Identity;

namespace Letterbook.Core.Models;

public class Account : IdentityUser<Guid>
{
    public ICollection<LinkedProfile> LinkedProfiles { get; set; } = new HashSet<LinkedProfile>();

    public Account()
    {
        base.Id = Uuid7.NewUuid7();
    }
    
    // In the future, Account might tie in to things like organizations or billing accounts

    // TODO(Account creation): https://github.com/Letterbook/Letterbook/issues/141
    public static Account CreateAccount(Uri baseUri, string email, string handle)
    {
        var profile = Profile.CreateIndividual(baseUri, handle);
        var account = new Account
        {
            Email = email,
            UserName = handle
        };
        profile.OwnedBy = account;
        account.LinkedProfiles.Add(new LinkedProfile(account, profile, ProfilePermission.All));
        
        return account;
    }

    public Account ShallowClone() => (Account)MemberwiseClone();
}