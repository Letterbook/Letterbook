using System.Net.Mime;
using System.Security.Claims;
using Letterbook.Core.Adapters;
using Letterbook.Core.Exceptions;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models;
using Letterbook.Core.Queries;
using Medo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace Letterbook.Core;

/// <summary>
/// The PostService is how you Post™
/// </summary>
public class PostService : IAuthzPostService, IPostService
{
	private readonly ILogger<PostService> _logger;
	private readonly CoreOptions _options;
	private readonly IDataAdapter _data;
	private readonly IPostEventPublisher _postEvents;
	private readonly IProfileEventPublisher _profileEvents;
	private readonly IApCrawlScheduler _crawler;
	private readonly IEnumerable<IContentSanitizer> _sanitizers;
	private IEnumerable<Claim> _claims = null!;
	private readonly Instrumentation _instrument;

	public PostService(ILogger<PostService> logger, IOptions<CoreOptions> options, Instrumentation instrumentation, IDataAdapter data,
		IPostEventPublisher postEvents, IProfileEventPublisher profileEvents, IApCrawlScheduler crawler,
		IEnumerable<IContentSanitizer> sanitizers)
	{
		_logger = logger;
		_data = data;
		_postEvents = postEvents;
		_profileEvents = profileEvents;
		_crawler = crawler;
		_sanitizers = sanitizers;
		_options = options.Value;
		_instrument = instrumentation;
	}

	public IQueryable<Post> Query(PostId id)
	{
		return _data.Posts(id);
	}

	public async Task<Post?> LookupPost(PostId id, bool withThread = true)
	{
		return withThread
			? await _data.Posts(id).WithThread().FirstOrDefaultAsync()
			: await _data.Posts(id).FirstOrDefaultAsync();
	}

	public async Task<Post?> LookupPost(ProfileId asProfile, Uri fediId, bool withThread = true)
	{
		return withThread
			? await _data.Posts(fediId).WithThread().FirstOrDefaultAsync()
			: await _data.Posts(fediId).FirstOrDefaultAsync();
	}

	public async Task<ThreadContext?> LookupThread(ProfileId asProfile, ThreadId id)
	{
		return await _data.Threads(id).Include(t => t.Posts).FirstOrDefaultAsync();
	}

	public async Task<ThreadContext?> LookupThread(ProfileId asProfile, Uri threadId)
	{
		return await _data.Threads(threadId).Include(t => t.Posts).FirstOrDefaultAsync();
	}

	public async Task<Post> DraftNote(ProfileId asProfile, string contentSource, PostId? inReplyToId = default)
	{
		var author = await _data.Profiles(asProfile).WithKeys().FirstOrDefaultAsync()
					 ?? throw CoreException.MissingData($"Couldn't find profile {asProfile}", typeof(Profile), asProfile);
		var post = new Post(_options);
		var note = new Note
		{
			SourceText = contentSource,
			SourceContentType = new ContentType(Content.PlainTextMediaType),
			Post = post,
			FediId = default!,
			Html = contentSource
		};
		post.Creators.Add(author);
		note.SetLocalFediId(_options);
		note.GeneratePreview();
		post.AddContent(note);

		return await Draft(asProfile, post, inReplyToId);
	}

	public async Task<Post> Draft(ProfileId asProfile, Post post, PostId? inReplyToId = default, bool publish = false)
	{
		using var span = _instrument.Span<PostService>();

		if (inReplyToId is { } parentId)
		{
			var parent = await _data.Posts(parentId).WithCommonFields().WithThread().FirstOrDefaultAsync()
						 ?? throw CoreException.MissingData($"Couldn't find post {parentId} to reply to", typeof(Post),
							 parentId);
			post.InReplyTo = parent;
			post.Thread = parent.Thread;
			post.Thread.Posts.Add(post);
		}

		var profiles = await _data.Profiles([asProfile, ..post.AddressedTo.Select(m => m.Subject.Id)]).WithRelation(asProfile)
			.ToDictionaryAsync(p => p.Id);
		post.ConvergeMentions(profiles);

		if (await _data.Profiles(asProfile).Include(p => p.Headlining).FirstOrDefaultAsync() is not { } author)
			throw CoreException.MissingData<Profile>($"Couldn't find profile {asProfile}", asProfile);
		post.Creators.Clear();
		post.Creators.Add(author);
		foreach (var content in post.Contents)
		{
			content.Sanitize(_sanitizers);
		}

		if (post.Audience.Intersect([Audience.Public, ..author.Headlining]).Any())
		{
			// Fill in the followers audience(s), if necessary. Also fixes reference equality if the audience was already included
			_logger.LogDebug("Normalizing {Audience} for followers from {Headliners}",
				post.Audience.Select(a => a.FediId),
				post.Creators.SelectMany(p => p.Headlining).Select(a => a.FediId));

			var followers = post.Creators.Select(Audience.Followers).ToHashSet()
				.ReplaceFrom(post.Creators.SelectMany(p => p.Headlining).ToHashSet()).ToHashSet();
			followers.UnionWith(post.Audience);
			post.Audience = followers;
			_logger.LogDebug("Normalized Audience for followers {Audience}", post.Audience.Select(a => a.FediId));
		}
		foreach (var audience in post.Audience)
		{
			// EF Core often incorrectly assumes the audience records are new, but that should never be true in this code path
			// Creating new audiences would be an explicit action, and the only time it happens incidentally is when creating new
			// Profiles
			_data.Update(audience);
		}

		if (publish) post.PublishedDate = DateTimeOffset.UtcNow;
		_data.Add(post);
		await _data.Commit();
		span?.AddTag("letterbook.post.audience", string.Join(",", post.Audience.Select(a => a.FediId)));
		span?.AddTag("letterbook.post.mentions", string.Join(",", post.AddressedTo.Select(m => m.Subject.Id)));
		await _postEvents.Created(post, (Uuid7)asProfile, _claims);
		if (publish) await _postEvents.Published(post, (Uuid7)asProfile, _claims);

		return post;
	}

	public async Task<Post> Update(ProfileId asProfile, PostId postId, Post post)
	{
		// TODO: authz
		// I think authz can be conveyed around the app with just a list of claims, as long as one of the claims is
		// a profile, right?
		var previous = await _data.Posts(postId).WithCommonFields().WithThread().FirstOrDefaultAsync()
					   ?? throw CoreException.MissingData($"Could not find existing post {post.Id} to update",
						   typeof(Post), postId);

		if (!(previous as IFederated).StrictEqual(post))
			throw CoreException.InvalidRequest("Input IDs don't match", $"{postId}", post);
		// var decision = _authz.Update(Enumerable.Empty<Claim>(), previous) // TODO: authz
		previous.Client = post.Client; // probably should come from an authz claim
		previous.InReplyTo = post.InReplyTo;

		previous.Audience = previous.Audience.ReplaceWith(post.Audience);
		previous.Contents = previous.Contents.ReplaceWith(post.Contents);

		var published = previous.PublishedDate != null;
		if (published) previous.UpdatedDate = DateTimeOffset.UtcNow;
		else previous.CreatedDate = DateTimeOffset.UtcNow;

		foreach (var content in previous.Contents)
		{
			content.Sanitize(_sanitizers);
		}


		_data.Update(previous);
		await _data.Commit();
		await _postEvents.Updated(previous, (Uuid7)asProfile, _claims);

		return previous;
	}

	public async Task Delete(ProfileId asProfile, PostId id)
	{
		var post = await _data.Posts(id).WithCommonFields().FirstOrDefaultAsync();
		if (post is null) return;
		// TODO: authz and thread root
		post.DeletedDate = DateTimeOffset.UtcNow;
		_data.Remove(post);
		await _data.Commit();
		await _postEvents.Deleted(post, (Uuid7)asProfile, _claims);
	}

	public async Task<Post> Publish(ProfileId asProfile, PostId id, bool localOnly = false)
	{
		var post = await _data.Posts(id).WithCommonFields().FirstOrDefaultAsync();
		if (post is null) throw CoreException.MissingData<Post>($"Can't find post {id} to publish", id);
		if (post.PublishedDate is not null)
			throw CoreException.Duplicate($"Tried to publish post {id} that is already published", id);
		post.PublishedDate = DateTimeOffset.UtcNow;
		post.CreatedDate = DateTimeOffset.UtcNow;

		_data.Update(post);
		await _data.Commit();
		await _postEvents.Published(post, (Uuid7)asProfile, _claims);
		return post;
	}

	public async Task<Post> AddContent(ProfileId asProfile, PostId postId, Content content)
	{
		var post = await _data.Posts(postId).WithCommonFields().FirstOrDefaultAsync()
				   ?? throw CoreException.MissingData<Post>("Can't find existing post to add content", postId);
		if (!post.FediId.HasLocalAuthority(_options))
			throw CoreException.WrongAuthority("Can't modify contents of remote post", post.FediId);
		content.SortKey ??= (post.Contents.Select(c => c.SortKey).Max() ?? -1) + 1;
		content.Sanitize(_sanitizers);
		post.Contents.Add(content);

		_data.Update(post);
		await _data.Commit();
		if (post.PublishedDate is not null)
		{
			post.UpdatedDate = DateTimeOffset.UtcNow;
			await _postEvents.Published(post, (Uuid7)asProfile, _claims);
		}
		await _postEvents.Updated(post, (Uuid7)asProfile, _claims);
		return post;
	}

	public async Task<Post> RemoveContent(ProfileId asProfile, PostId postId, Uuid7 contentId)
	{
		var post = await _data.Posts(postId).WithCommonFields().FirstOrDefaultAsync()
				   ?? throw CoreException.MissingData<Post>("Can't find existing post to remove content", postId);
		if (!post.FediId.HasLocalAuthority(_options))
			throw CoreException.WrongAuthority("Can't modify contents of remote post", post.FediId);
		var content = post.Contents.FirstOrDefault(c => c.Id == contentId);
		if (content is null)
		{
			_logger.LogWarning("Tried to remove content {ContentId} that doesn't exist from post {PostId}", contentId, postId);
			return post;
		}

		post.Contents.Remove(content);

		if (post.PublishedDate is not null)
		{
			post.UpdatedDate = DateTimeOffset.UtcNow;
			await _postEvents.Published(post, (Uuid7)asProfile, _claims);
		}

		_data.Remove(content);
		_data.Update(post);
		await _data.Commit();
		await _postEvents.Updated(post, (Uuid7)asProfile, _claims);
		return post;
	}

	public async Task<Post> UpdateContent(ProfileId asProfile, PostId postId, Uuid7 contentId, Content content)
	{
		var post = await _data.Posts(postId).WithCommonFields().FirstOrDefaultAsync()
				   ?? throw CoreException.MissingData<Post>("Can't find existing post to add content", postId);
		if (!post.FediId.HasLocalAuthority(_options))
			throw CoreException.WrongAuthority("Can't modify contents of remote post", post.FediId);
		content.Id = contentId.ToGuid();
		content.SetLocalFediId(_options);
		if (post.Contents.FirstOrDefault(content) is not { } original)
		{
			var details = new Dictionary<string, object>();
			details.Add("post", postId);
			details.Add("content", contentId);
			throw CoreException.InvalidRequest("Can't find content to update in post", details);
		}

		original.UpdateFrom(content);
		original.Sanitize(_sanitizers);

		if (post.PublishedDate is not null)
		{
			post.UpdatedDate = DateTimeOffset.UtcNow;
			await _postEvents.Published(post, (Uuid7)asProfile, _claims);
		}

		_data.Update(post);
		await _data.Commit();
		await _postEvents.Updated(post, (Uuid7)asProfile, _claims);
		return post;
	}

	public async Task<IEnumerable<Post>> ReceiveCreate(IEnumerable<Post> posts)
	{
		// If we don't already recognize this actor, then we probably can't trust the activity
		// crawl the actor and posts, instead
		if (_claims.FirstOrDefault(c => c.Type == ApplicationClaims.Actor)?.Value is not { } actorId)
		{
			foreach (var post in posts)
			{
				await _crawler.CrawlPost(default, post.FediId, 1);
			}

			return [];
		}
		if(await _data.Profiles(new Uri(actorId)).SingleOrDefaultAsync() is not { } actor)
		{
			await _crawler.CrawlProfile(default, new Uri(actorId));
			foreach (var post in posts)
			{
				await _crawler.CrawlPost(default, post.FediId);
			}
			return [];
		}

		posts = posts.ToList();
		// posts = posts.Concat(posts.Select(p => p.InReplyTo).WhereNotNull()).ToList();
		// TODO: authorization

		// lookup posts we already know about, because it's very likely for new posts to reference old objects
		// or to receive multiple Create activities for the same object
		var knownPosts = await ConvergePosts(posts);
		var threads = await ConvergeThreads(posts);
		var profiles = posts.SelectMany(p => p.Creators)
			.Concat(posts.SelectMany(p => p.AddressedTo).Select(m => m.Subject))
			.DistinctBy(p => p.FediId)
			.ToList();
		var knownProfiles = await ConvergeProfiles(profiles);
		var knownAudience = await ConvergeAudience(posts);

		var pendingPosts = posts.Concat(posts.Select(p => p.InReplyTo).WhereNotNull())
			.Where(p => !knownPosts.ContainsKey(p.FediId))
			.DistinctBy(p => p.FediId)
			.ToDictionary(post => post.FediId);

		foreach (var post in pendingPosts.Values)
		{
			if (post.InReplyTo != null && knownPosts.TryGetValue(post.InReplyTo.FediId, out var value))
				post.InReplyTo = value;
			else if (post.InReplyTo != null && pendingPosts.TryGetValue(post.InReplyTo.FediId, out value))
				post.InReplyTo = value;

			post.Creators = post.Creators.ReplaceFrom(knownProfiles.Values, FediIdCompare.Instance);
			foreach (var mention in post.AddressedTo)
			{
				if (knownProfiles.TryGetValue(mention.Subject.FediId, out var mentioned))
					mention.Subject = mentioned;
			}

			SelectThreadHeuristically(post, threads, knownPosts);
		}

		foreach (var pendingPost in pendingPosts.Values)
		{
			_data.Add(pendingPost);
			if (pendingPost.Audience.Contains(Audience.Public))
				_data.Update(pendingPost.Audience.First(a => a == Audience.Public));
		}
		await _data.Commit();

		foreach (var post in pendingPosts.Values)
		{
			await _postEvents.Created(post, (Uuid7)actor.Id, _claims);
			await _postEvents.Published(post, (Uuid7)actor.Id, _claims);
		}

		foreach (var profile in profiles.Where(p => !knownProfiles.ContainsKey(p.FediId)))
		{
			await _profileEvents.Created(profile);
			await _crawler.CrawlProfile(default, profile.FediId);
		}

		foreach (var referencedPost in pendingPosts.Select(p => p.Value.InReplyTo).WhereNotNull().Where(p => !knownPosts.ContainsKey(p.FediId)))
		{
			await _crawler.CrawlPost(default, referencedPost.FediId);
		}

		return pendingPosts.Values.Concat(knownPosts.Values.Where(p => posts.Contains(p)));
	}

	public async Task<IEnumerable<Post>> ReceiveUpdate(IEnumerable<Post> posts)
	{
		if (_claims.FirstOrDefault(c => c.Type == ApplicationClaims.Actor)?.Value is not { } actorId)
		{
			foreach (var post in posts)
			{
				await _crawler.CrawlPost(default, post.FediId, 1);
			}

			return [];
		}
		if(await _data.Profiles(new Uri(actorId)).SingleOrDefaultAsync() is not { } actor)
		{
			await _crawler.CrawlProfile(default, new Uri(actorId));
			foreach (var post in posts)
			{
				await _crawler.CrawlPost(default, post.FediId);
			}
			return [];
		}

		posts = posts.ToList();
		var knownPosts = await _data.Posts(posts.Select(p => p.FediId).ToArray())
			.Include(p => p.Thread)
			.Include(p => p.InReplyTo)
			.Include(p => p.Audience)
			.Include(p => p.AddressedTo).ThenInclude(m => m.Subject)
			.Include(p => p.Contents)
			.Include(p => p.Creators)
			.AsSplitQuery()
			.ToDictionaryAsync(p => p.FediId);
		// Unknown posts need to go through create logic
		var updatedPosts = (await ReceiveCreate(posts.Where(post => !knownPosts.ContainsKey(post.FediId)))).ToList();

		var pendingPosts = posts.Where(p => knownPosts.ContainsKey(p.FediId)).ToList();
		if (pendingPosts.Count == 0)
			return updatedPosts;

		var profiles = posts.SelectMany(p => p.Creators)
			.Concat(posts.SelectMany(p => p.AddressedTo).Select(m => m.Subject))
			.DistinctBy(p => p.FediId)
			.ToList();
		var knownProfiles = await ConvergeProfiles(profiles);
		var knownAudience = await ConvergeAudience(pendingPosts);

		foreach (var pending in pendingPosts)
		{
			var post = knownPosts[pending.FediId];

			foreach (var mention in pending.AddressedTo)
			{
				mention.Subject = !knownProfiles.TryGetValue(mention.Subject.FediId, out var knownSubject)
					? mention.Subject
					: knownSubject;
			}

			foreach (var m in pending.AddressedTo)
			{
				m.Subject = !knownProfiles.TryGetValue(m.Subject.FediId, out var knownSubject)
					? m.Subject
					: knownSubject;
			}

			var knownContents = post.Contents.ToDictionary(c => c.FediId);

			post.Contents = pending.Contents.Select(rc =>
			{
				if (!knownContents.TryGetValue(rc.FediId, out var known)) return rc;
				known.UpdateFrom(rc);
				return known;
			}).ToList();
			post.AddressedTo = pending.AddressedTo;
			post.Audience = pending.Audience.ReplaceFrom(knownAudience.Values);
			post.InReplyTo = pending.InReplyTo == null ? null : knownPosts[pending.InReplyTo.FediId].InReplyTo;
			post.UpdatedDate = pending.UpdatedDate;
			post.Client = pending.Client;
			post.Summary = pending.Summary;
			post.Preview = pending.Preview;
			post.Likes = pending.Likes;
			post.Replies = pending.Replies;
			post.Shares = pending.Shares;
		}

		await _data.Commit();

		foreach (var post in pendingPosts)
		{
			await _postEvents.Updated(post, (Uuid7)actor.Id, _claims);
		}

		foreach (var pending in profiles.Except(knownProfiles.Values))
		{
			await _crawler.CrawlProfile(default, pending.FediId);
		}

		return updatedPosts.Concat(pendingPosts);
	}

	public async Task<Post> ReceiveUpdate(Uri post)
	{
		throw new NotImplementedException();
	}

	public async Task<IEnumerable<Post>> ReceiveDelete(IEnumerable<Uri> items)
	{
		// If we don't recognize this actor then there's definitely nothing for us to do
		if (_claims.FirstOrDefault(c => c.Type == ApplicationClaims.Actor)?.Value is not { } actorId)
			return [];
		if (await _data.Profiles(new Uri(actorId)).SingleOrDefaultAsync() is not { } actor)
			return [];

		var posts = new List<Post>(items.Count());
		await foreach (var post in _data.Posts(items.ToArray()).AsAsyncEnumerable())
		{
			post.DeletedDate ??= DateTimeOffset.UtcNow;
			posts.Add(post);
		}

		await _data.Commit();

		foreach (var post in posts)
		{
			await _postEvents.Deleted(post, (Uuid7)actor.Id, _claims);
		}

		return posts;
	}

	public async Task<Post> ReceiveAnnounce(Post post, Uri announcedBy)
	{
		throw new NotImplementedException();
	}

	public async Task<Post> ReceiveAnnounce(Uri post, Uri announcedBy)
	{
		throw new NotImplementedException();
	}

	public async Task<Post> ReceiveUndoAnnounce(Uri post, Uri likedBy)
	{
		throw new NotImplementedException();
	}

	public async Task<Post> ReceiveLike(Uri post, Uri likedBy)
	{
		throw new NotImplementedException();
	}

	public async Task<Post> ReceiveUndoLike(Uri post, Uri likedBy)
	{
		throw new NotImplementedException();
	}

	public async Task Share(ProfileId asProfile, PostId id)
	{
		throw new NotImplementedException();
	}

	public async Task Like(ProfileId asProfile, PostId id)
	{
		throw new NotImplementedException();
	}

	public IAuthzPostService As(IEnumerable<Claim> claims)
	{
		_claims = claims;

		return this;
	}

	/***
	 * Support methods
	 */

	/// <summary>
	/// Deduplicate and replace posts with Post values from the db
	/// </summary>
	/// <param name="posts"></param>
	/// <returns></returns>
	private async Task<Dictionary<Uri, Post>> ConvergePosts(IEnumerable<Post> posts)
	{
		var postIds = posts.Select(p => p.FediId)
			.Concat(posts.Select(p => p.InReplyTo).WhereNotNull().Select(p => p.FediId))
			.ToArray();
		return await _data.Posts(postIds)
			.Include(post => post.Thread)
			.ToDictionaryAsync(p => p.FediId);
	}

	/// <summary>
	/// Deduplicate and replace profiles with Profile values from the db
	/// </summary>
	/// <param name="profiles"></param>
	/// <returns></returns>
	private async Task<Dictionary<Uri, Profile>> ConvergeProfiles(IEnumerable<Profile> profiles)
	{
		return await _data.Profiles(profiles.Select(p => p.FediId).ToArray()).ToDictionaryAsync(p => p.FediId);
	}

	/// <summary>
	/// Collects and deduplicates threads attached to posts with ThreadContext values from the db
	/// </summary>
	/// <param name="posts"></param>
	/// <returns></returns>
	private async Task<Dictionary<Uri, ThreadContext>> ConvergeThreads(IEnumerable<Post> posts)
	{
		var relatives = posts.ToDictionary(p => p, p => new List<Post>());
		foreach (var post in posts)
		{
			if (post.InReplyTo is not { } replyParent) continue;
			if (!relatives.TryGetValue(replyParent, out var list))
			{
				list = [];
				relatives[replyParent] = list;
			}
			list.Add(post);
		}

		foreach (var (post, list) in relatives)
		{
			// This can happen when the post was scaffolded from just a Uri, likely because it was referenced in a reply
			// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
			post.Thread ??= list.Select(p => p.Thread).WhereNotNull().FirstOrDefault() ?? new ThreadContext
			{
				RootId = post.Id,
				FediId = post.FediId,
			};
		}

		var _threads = posts.Select(p => p.Thread).ToList();
		var possibleThreadIds = _threads.Select(t => t.Heuristics?.Root)
			.Concat(_threads.Select(t => t.Heuristics?.Context))
			.Concat(_threads.Select(t => t.Heuristics?.Target))
			.Concat(_threads.Select(t => t.FediId))
			.Distinct()
			.WhereNotNull()
			.ToArray();
		var threads = posts.Select(p => p.Thread).Where(t => t.FediId != null)
			.DistinctBy(t => t.FediId)
			.ToDictionary(t => t.FediId!);
		var knownThreads = await _data.Threads(possibleThreadIds).ToListAsync();
		foreach (var thread in knownThreads)
		{
			threads[thread.FediId!] = thread;
		}

		return threads;
	}

	private async Task<Dictionary<Uri, Audience>> ConvergeAudience(IEnumerable<Post> posts)
	{
		var audienceIds = posts.SelectMany(p => p.Audience).Select(a => a.FediId)
			.Concat(posts.SelectMany(p => p.AddressedTo).Select(m => m.Subject.FediId));
		var knownAudience = await _data.AllAudience()
			.Where(a => audienceIds.Contains(a.FediId))
			.Distinct()
			.ToDictionaryAsync(a => a.FediId);
		foreach (var post in posts)
		{
			FixupAudience(post, knownAudience);
		}


		return knownAudience;
	}

	/// <summary>
	/// Find audience refs in mentions and move them to Audience where they belong
	/// </summary>
	/// <param name="post"></param>
	/// <exception cref="NotImplementedException"></exception>
	private void FixupAudience(Post post, Dictionary<Uri, Audience> audiences)
	{
		var found = post.AddressedTo.Where(m => audiences.ContainsKey(m.Subject.FediId)).ToList();
		post.AddressedTo = post.AddressedTo.Where(m => !found.Contains(m)).ToHashSet();
		post.Audience = post.Audience.ReplaceFrom(audiences.Values);
		foreach (var mentionedAudience in found)
		{
			post.Audience.Add(audiences[mentionedAudience.Subject.FediId]);
		}
	}

	private static void SelectThreadHeuristically(Post post, Dictionary<Uri, ThreadContext> threads,
		Dictionary<Uri, Post> knownPosts)
	{
		var threadHeuristics = post.Thread.Heuristics ?? new Heuristics();
		if (threadHeuristics.NewThread)
		{
			if (post.Thread.FediId is { } id && threads.TryGetValue(id, out var thread))
				post.Thread = thread;
			return;
		}
		var threadId = threadHeuristics.Root ?? threadHeuristics.Context ?? threadHeuristics.Target ?? post.Thread.FediId;

		// TODO: reply controls here
		if (threadId is { } cId && threads.TryGetValue(cId, out var c))
		{
			post.Thread = c;
			post.Thread.Posts.Add(post);
		}
		else if (post.InReplyTo is {} r && knownPosts.TryGetValue(r.FediId, out var parent))
		{
			post.Thread = parent.Thread;
			post.Thread.Posts.Add(post);
		}
		else if (threadId is not null)
			post.Thread.FediId ??= threadId;
	}
}