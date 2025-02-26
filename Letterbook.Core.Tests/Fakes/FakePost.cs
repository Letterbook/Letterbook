using Bogus;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models;
using Xunit.Sdk;

namespace Letterbook.Core.Tests.Fakes;

public class FakePost : Faker<Post>
{
	private IEnumerable<Profile> _creators;
	private string _authority;

	public FakePost(string authority) : this(new FakeProfile(authority).Generate())
	{
	}

	public FakePost(Profile creator, bool draft = false, CoreOptions? opts = null) : this(new List<Profile> { creator }, 1, opts)
	{
		if (draft)
		{
			RuleFor(p => p.PublishedDate, (_, _) => null);
		}
	}

	public FakePost(IEnumerable<Profile> creators, int contents, CoreOptions? opts)
	{
		_creators = creators;
		_authority = _creators.First().FediId.Authority;
		RuleFor(p => p.Id, faker => new PostId(faker.Random.Guid7()));
		RuleFor(p => p.FediId, (faker, post) => faker.FediId(_authority, "post", post.Id.Id));
		RuleFor(p => p.Creators, () => _creators);
		RuleFor(p => p.CreatedDate, faker => faker.Date.Recent().ToUniversalTime());
		RuleFor(p => p.PublishedDate, (_, post) => post.CreatedDate);
		RuleFor(p => p.LastSeenDate, (_, post) => post.CreatedDate);
		RuleFor(p => p.Thread, (faker, post) => new ThreadContext
		{
			FediId = faker.FediId(_authority, "thread"),
			RootId = post.Id
		});
		RuleFor(p => p.Authority, (_, post) => post.FediId.GetAuthority());
		RuleFor(p => p.Hostname, (_, post) => post.FediId.Host);
		RuleFor(p => p.Audience, (faker, post) =>
		{
			var audience = post.Creators.SelectMany(c => c.Headlining);
			if (faker.Random.Bool())
				audience = audience.Append(Audience.Public);

			return audience.ToList();
		});
		RuleFor(p => p.Likes, (_, post) => new Uri(post.FediId + "/likes"));
		RuleFor(p => p.Replies, (_, post) => new Uri(post.FediId + "/replies"));
		RuleFor(p => p.Shares, (_, post) => new Uri(post.FediId + "/shares"));

		FinishWith((_, post) =>
		{
			var note = new FakeNote(post, opts);
			foreach (var n in note.Generate(contents))
			{
				post.AddContent(n);
			}

			post.Thread.Posts.Add(post);
			post.Thread.RootId = post.Id;
		});
	}

	public FakePost(Profile creator, Post inReplyTo) : this(creator)
	{
		RuleFor(post => post.Thread, () => inReplyTo.Thread);
		RuleFor(post => post.InReplyTo, () => inReplyTo);
		FinishWith((faker, post) =>
		{
			foreach (var c in inReplyTo.Creators)
			{
				post.Mention(c, MentionVisibility.To);
			}

			post.Thread.Posts.Add(post);

			post.AddContent(new FakeNote(post, null).Generate());
		});
	}
}