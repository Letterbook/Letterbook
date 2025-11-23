using Bogus;
using Letterbook.Core.Models;

namespace Letterbook.Core.Tests.Fakes;

public sealed class FakeReport : Faker<ModerationReport>
{
	public FakeReport() : this(new FakeProfile(), new FakeProfile(), new CoreOptions()) { }

	public FakeReport(Profile reporter, Profile subject, CoreOptions opts)
	{
		CustomInstantiator(f => new ModerationReport(opts, f.Lorem.Sentence()));

		RuleFor(r => r.Reporter, () => reporter);
		RuleFor(r => r.Subjects, () => [subject]);
		RuleFor(r => r.Created, (faker) => faker.Date.RecentOffset(5));
		RuleFor(r => r.Updated, (faker, report) => faker.Random.Bool() ? report.Created : faker.Date.BetweenOffset(report.Created, DateTimeOffset.UtcNow));
		RuleFor(r => r.Closed, (faker, report) => faker.Random.Bool(0.1f) ? DateTimeOffset.UtcNow : DateTimeOffset.MaxValue);
	}

	public FakeReport(FakeProfile reporter, FakeProfile subject, CoreOptions opts)
	{
		CustomInstantiator(f => new ModerationReport(opts, f.Lorem.Sentence()));

		RuleFor(r => r.Reporter, (faker ) => faker.Random.Bool() ? reporter.Generate() : null);
		RuleFor(r => r.Subjects, () => subject.Generate(1));
		RuleFor(r => r.Created, (faker) => faker.Date.RecentOffset(5));
		RuleFor(r => r.Updated, (faker, report) => faker.Random.Bool() ? report.Created : faker.Date.BetweenOffset(report.Created, DateTimeOffset.UtcNow));
		RuleFor(r => r.Closed, (faker, report) => faker.Random.Bool(0.1f) ? DateTimeOffset.UtcNow : DateTimeOffset.MaxValue);
	}

	public FakeReport(Profile reporter, Profile subject)
	{
		CustomInstantiator(f =>
		{
			var id = new ModerationReportId(f.Random.Uuid7());
			var fediId = new Uri(reporter.FediId, $"/report/{id}");
			return new ModerationReport
			{
				Id = id,
				FediId = fediId,
				Summary = f.Lorem.Sentence(),
				Context = new ThreadContext
				{
					RootId = default(PostId),
					FediId = fediId,
				},
				Reporter = reporter,
				Subjects = [subject],
			};
		});
	}

	public FakeReport(Profile reporter, Post post)
	{
		CustomInstantiator(f =>
		{
			var id = new ModerationReportId(f.Random.Uuid7());
			var fediId = new Uri(reporter.FediId, $"/report/{id}");
			return new ModerationReport
			{
				Id = id,
				FediId = fediId,
				Summary = f.Lorem.Sentence(),
				Context = new ThreadContext
				{
					RootId = default(PostId),
					FediId = fediId,
				},
				RelatedPosts = [post]
			};
		});

		RuleFor(r => r.Reporter, () => reporter);
		RuleFor(r => r.Subjects, () => post.Creators);
		RuleFor(r => r.RelatedPosts, () => [post]);
		RuleFor(r => r.Created, () => DateTimeOffset.UtcNow);
		RuleFor(r => r.Updated, () => DateTimeOffset.UtcNow);
	}
}