using Bogus;
using Letterbook.Core.Models;

namespace Letterbook.Core.Tests.Fakes;

public sealed class FakeReport : Faker<ModerationReport>
{
	public FakeReport() : this(new FakeProfile().Generate(), new FakeProfile().Generate(), new CoreOptions()) { }

	public FakeReport(Profile reporter, Profile subject, CoreOptions opts)
	{
		CustomInstantiator(f => new ModerationReport(opts, f.Lorem.Sentence()));

		RuleFor(r => r.Reporter, () => reporter);
		RuleFor(r => r.Subjects, () => [subject]);
		RuleFor(r => r.Created, () => DateTimeOffset.UtcNow);
		RuleFor(r => r.Updated, () => DateTimeOffset.UtcNow);
	}
}