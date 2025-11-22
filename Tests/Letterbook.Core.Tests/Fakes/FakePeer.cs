using Bogus;
using Letterbook.Core.Models;

namespace Letterbook.Core.Tests.Fakes;

public class FakePeer : Faker<Peer>
{
	public FakePeer(bool hasRestriction = true)
	{
		CustomInstantiator(faker => new Peer(faker.Internet.DomainName()));
		RuleFor(p => p.PublicRemark, faker => faker.Lorem.Sentences(faker.Random.Int(0, 3)));
		RuleFor(p => p.PrivateComment, faker => faker.Lorem.Sentence(1, 2));
		if (hasRestriction)
			RuleFor(p => p.Restrictions, faker => new Dictionary<Restrictions, DateTimeOffset>()
			{
				{ faker.PickRandomWithout(Restrictions.Warn), faker.Date.FutureOffset() }
			});
	}
}