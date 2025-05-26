using System.Security.Claims;
using Bogus;
using Letterbook.Core;
using Letterbook.Core.Tests.Fakes;
using Medo;
using Microsoft.Extensions.Options;

namespace Letterbook.Web.Mocks;

public class MockPostService : IPostService, IAuthzPostService
{
	// 000000000000000000000007b
	private static readonly Models.PostId notFoundId = new Uuid7(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0 });
	// 00000000000000000000000en
	private static readonly Models.PostId soloId = new Uuid7(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0 });
	// 00000000000000000000000u9
	private static readonly Models.PostId parentsOnly = new Uuid7(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 0 });
	// 00000000000000000000001oi
	private static readonly Models.PostId repliesOnly = new Uuid7(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 0 });
	// 00000000000000000000003c1
	private static readonly Models.PostId midThread = new Uuid7(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 16, 0 });

	private PostService _postService;
	private FakePost _fakePost;
	private Faker _fakes;
	public MockPostService(IOptions<CoreOptions> opts, PostService postService)
	{
		_fakes = new Faker();
		_fakePost = new FakePost(opts.Value.DomainName);
		_postService = postService;
	}

	public IAuthzPostService As(IEnumerable<Claim> claims)
	{
		_postService.As(claims);
		return this;
	}

	public async Task<Models.Post?> LookupPost(Models.PostId id, bool withThread = true)
	{
		if (id == notFoundId)
			return default;
		if (id == soloId)
			return _fakePost.Generate();
		if (id == repliesOnly)
		{
			var post = _fakePost.Generate();
			return GenerateReplies(post, _fakes.Random.Int(1, 4));
		}
		if (id == parentsOnly)
		{
			var post = _fakePost.Generate();
			return GenerateAncestors(post, _fakes.Random.Int(1, 4));
		}
		if (id == midThread)
		{
			var post = _fakePost.Generate();
			return GenerateReplies(GenerateAncestors(post, _fakes.Random.Int(1, 4)), _fakes.Random.Int(1, 4));
		}
		return await _postService.LookupPost(id, withThread);
	}

	private Models.Post GenerateReplies(Models.Post post, int depth)
	{
		var p = post;
		while (depth-- > 0)
		{
			foreach (var reply in _fakePost.Generate(_fakes.Random.Int(1, 4)))
			{
				p.RepliesCollection.Insert(0, reply);
			}

			p = _fakes.PickRandom(p.RepliesCollection);
		}

		return post;
	}
	private Models.Post GenerateAncestors(Models.Post post, int depth)
	{
		var p = post;
		while (depth-- > 0)
		{
			p.InReplyTo = _fakePost.Generate();
			p = p.InReplyTo;
		}

		return post;
	}

	public Task<Models.Post?> LookupPost(Models.ProfileId asProfile, Uri id, bool withThread = true)
	{
		return _postService.LookupPost(asProfile, id, withThread);
	}
	public Task<Models.ThreadContext?> LookupThread(Models.ProfileId asProfile, Models.ThreadId id)
	{
		return _postService.LookupThread(asProfile, id);
	}
	public Task<Models.ThreadContext?> LookupThread(Models.ProfileId asProfile, Uri id)
	{
		return _postService.LookupThread(asProfile, id);
	}
	public Task<Models.Post> DraftNote(Models.ProfileId asProfile, string contentSource, Models.PostId? inReplyToId = default)
	{
		return _postService.DraftNote(asProfile, contentSource, inReplyToId);
	}
	public Task<Models.Post> Draft(Models.ProfileId asProfile, Models.Post post, Models.PostId? inReplyToId = default, bool publish = false)
	{
		return _postService.Draft(asProfile, post, inReplyToId, publish);
	}
	public Task<Models.Post> Update(Models.ProfileId asProfile, Models.PostId postId, Models.Post post)
	{
		return _postService.Update(asProfile, postId, post);
	}
	public Task Delete(Models.ProfileId asProfile, Models.PostId id)
	{
		return _postService.Delete(asProfile, id);
	}
	public Task Share(Models.ProfileId asProfile, Models.PostId id)
	{
		return _postService.Share(asProfile, id);
	}
	public Task Like(Models.ProfileId asProfile, Models.PostId id)
	{
		return _postService.Like(asProfile, id);
	}
	public Task<Models.Post> AddContent(Models.ProfileId asProfile, Models.PostId postId, Models.Content content)
	{
		return _postService.AddContent(asProfile, postId, content);
	}
	public Task<Models.Post> RemoveContent(Models.ProfileId asProfile, Models.PostId postId, Uuid7 contentId)
	{
		return _postService.RemoveContent(asProfile, postId, contentId);
	}
	public Task<Models.Post> UpdateContent(Models.ProfileId asProfile, Models.PostId postId, Uuid7 contentId, Models.Content content)
	{
		return _postService.UpdateContent(asProfile, postId, contentId, content);
	}
	public Task<Models.Post> Publish(Models.ProfileId asProfile, Models.PostId id, bool localOnly = false)
	{
		return _postService.Publish(asProfile, id, localOnly);
	}
	public Task<IEnumerable<Models.Post>> ReceiveCreate(IEnumerable<Models.Post> posts)
	{
		return _postService.ReceiveCreate(posts);
	}
	public Task<IEnumerable<Models.Post>> ReceiveUpdate(IEnumerable<Models.Post> posts)
	{
		return _postService.ReceiveUpdate(posts);
	}
	public Task<Models.Post> ReceiveUpdate(Uri post)
	{
		return _postService.ReceiveUpdate(post);
	}
	public Task<IEnumerable<Models.Post>> ReceiveDelete(IEnumerable<Uri> items)
	{
		return _postService.ReceiveDelete(items);
	}
	public Task<Models.Post> ReceiveAnnounce(Models.Post post, Uri announcedBy)
	{
		return _postService.ReceiveAnnounce(post, announcedBy);
	}
	public Task<Models.Post> ReceiveAnnounce(Uri post, Uri announcedBy)
	{
		return _postService.ReceiveAnnounce(post, announcedBy);
	}
	public Task<Models.Post> ReceiveUndoAnnounce(Uri post, Uri likedBy)
	{
		return _postService.ReceiveUndoAnnounce(post, likedBy);
	}
	public Task<Models.Post> ReceiveLike(Uri post, Uri likedBy)
	{
		return _postService.ReceiveLike(post, likedBy);
	}
	public Task<Models.Post> ReceiveUndoLike(Uri post, Uri likedBy)
	{
		return _postService.ReceiveUndoLike(post, likedBy);
	}
}