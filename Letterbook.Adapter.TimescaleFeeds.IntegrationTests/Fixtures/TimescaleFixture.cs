using Bogus;
using Letterbook.Adapter.TimescaleFeeds._Tests.Fixtures;
using Letterbook.Adapter.TimescaleFeeds.EntityModels;
using Letterbook.Core.Models;
using Letterbook.Core.Tests.Fakes;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace Letterbook.Adapter.TimescaleFeeds.IntegrationTests.Fixtures;

public class TimescaleDataFixture<T> : TimescaleFixture<T>
{
	private readonly Faker _faker = new();

	public List<Profile> Profiles { get; } = [];
	public List<Post> Posts { get; } = [];
	public List<TimelinePost> Timeline { get; set; } = [];
	public int Seed { get; init; }

	public TimescaleDataFixture(IMessageSink sink)
	{
		Seed = Init.WithSeed();
		InitTestData();

		using (FeedsContext context = CreateContext())
		{
			context.Database.EnsureDeleted();
			context.Database.Migrate();

			// populate the db with test data
			// context.AddRange();
			context.SaveChanges();
		}
	}

	private void InitTestData()
	{
		var fakeProfile = new FakeProfile("letterbook.example");
		Profiles.AddRange(fakeProfile.Generate(3));

		while (Posts.Count < 1000)
		{
			if (_faker.Random.Bool(0.1f))
				Profiles.AddRange(fakeProfile.Generate(_faker.Random.Number(1, 3)));
			Posts.AddRange(GeneratePosts());
		}

		var timeline = TimelinePost
			.Denormalize(Posts)
			.Select((post, i) =>
			{
				post.Time = DateTimeOffset.UtcNow - TimeSpan.FromMilliseconds(10 * i + _faker.Random.Number(0, 10));
				return post;
			})
			.OrderBy(p => p.Time)
			.ToList();
		Timeline.AddRange(timeline);
	}


	private IEnumerable<Post> GeneratePosts()
	{
		var creator = _faker.PickRandom(Profiles);
		Post? inReplyTo = default;
		if (Posts.Count != 0 && _faker.Random.Bool(0.6f))
			inReplyTo = _faker.PickRandom(Posts);

		var fake = inReplyTo == null
			? new FakePost(creator)
			: new FakePost(creator, inReplyTo);

		var count = inReplyTo == null
			? _faker.Random.Number(1, 4)
			: _faker.Random.Number(3, 15);

		return fake.Generate(count);
	}
}