using Bogus;
using Letterbook.Core.Models;

namespace Letterbook.Core.Tests.Fakes;

public class FakeAccount : Faker<Account>
{
    public FakeAccount()
    {
        CustomInstantiator(faker =>
        {
            var uri = faker.Internet.Url();
            var profile = new FakeProfile(uri).Generate();
            var account = new Account()
            {
                Email = faker.Internet.Email(),
                UserName = faker.Internet.UserName()
            };
            profile.OwnedBy = account;
            account.LinkedProfiles.Add(new LinkedProfile(account, profile, ProfilePermission.All));
            return account;
        });
    }

    public FakeAccount(bool empty = true)
    {
        
        RuleFor(a => a.Email, faker => faker.Internet.Email());
        RuleFor(a => a.UserName, faker => faker.Internet.UserName());
    }
}