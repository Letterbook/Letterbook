using Bogus;
using Letterbook.Core.Models;

namespace Letterbook.Core.Tests.Fakes;

public class FakeAccount : Faker<Account>
{
	public FakeAccount(bool withProfile = true)
	{
		base.CustomInstantiator(faker =>
		{
			var uri = faker.Internet.Url();
			var account = new Account()
			{
				Email = faker.Internet.Email(),
				UserName = faker.Internet.UserName()
			};
			if (!withProfile) return account;

			var profile = new FakeProfile(uri).Generate();
			profile.OwnedBy = account;
			var link = new ProfileAccess(account, profile, ProfilePermission.All);
			account.LinkedProfiles.Add(link);
			profile.Accessors.Add(link);
			return account;
		});
	}
}