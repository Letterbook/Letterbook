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