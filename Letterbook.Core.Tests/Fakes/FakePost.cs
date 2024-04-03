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
	{ }

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
		_authority = _creators.First().Authority;
		RuleFor(p => p.Id, faker => faker.Random.Guid7());
		RuleFor(p => p.FediId, faker => faker.FediId(_authority, "post"));
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
				post.AddressedTo.Add(Mention.To(c));
			}
			post.Thread.Posts.Add(post);

			post.AddContent(new FakeNote(post, null).Generate());
		});
	}

}