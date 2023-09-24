using Bogus;
using Letterbook.Core.Models;

namespace Letterbook.Core.Tests.Fakes;

public class FakeAccount : Faker<Account>
{
    public FakeAccount()
    {
        CustomInstantiator(faker =>
            Account.CreateAccount(new Uri(faker.Internet.Url()), faker.Internet.Email(), faker.Internet.UserName()));
    }

    public FakeAccount(bool empty = true)
    {
        
        RuleFor(a => a.Email, faker => faker.Internet.Email());
        RuleFor(a => a.UserName, faker => faker.Internet.UserName());
    }
}