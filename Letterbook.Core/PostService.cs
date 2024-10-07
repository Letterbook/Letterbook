using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using Letterbook.Core.Adapters;
using Letterbook.Core.Exceptions;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models;
using Medo;
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
	private readonly IDataAdapter _posts;
	private readonly IPostEventPublisher _postEventPublisher;
	private readonly IActivityPubClient _apClient;
	private readonly IEnumerable<IContentSanitizer> _sanitizers;
	private Uuid7 _profileId;
	private IEnumerable<Claim> _claims = null!;
	private readonly Instrumentation _instrument;

	public PostService(ILogger<PostService> logger, IOptions<CoreOptions> options, Instrumentation instrumentation,
		IDataAdapter posts, IPostEventPublisher postEventPublisher, IActivityPubClient apClient, IEnumerable<IContentSanitizer> sanitizers)
	{
		_logger = logger;
		_posts = posts;
		_postEventPublisher = postEventPublisher;
		_apClient = apClient;
		_sanitizers = sanitizers;
		_options = options.Value;
		_instrument = instrumentation;
	}

	public async Task<Post?> LookupPost(Uuid7 id, bool withThread = true)
	{
		return withThread
			? await _posts.LookupPostWithThread(id)
			: await _posts.LookupPost(id);
	}

	public async Task<Post?> LookupPost(Uri fediId, bool withThread = true)
	{
		return withThread
			? await _posts.LookupPostWithThread(fediId)
			: await _posts.LookupPost(fediId);
	}

	public async Task<ThreadContext?> LookupThread(Uuid7 threadId)
	{
		return await _posts.LookupThread(threadId);
	}

	public async Task<ThreadContext?> LookupThread(Uri threadId)
	{
		return await _posts.LookupThread(threadId);
	}

	public async Task<Post> DraftNote(Uuid7 authorId, string contentSource, Uuid7? inReplyToId = default)
	{
		var author = await _posts.LookupProfile(authorId)
					 ?? throw CoreException.MissingData($"Couldn't find profile {authorId}", typeof(Profile), authorId);
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

		return await Draft(post, inReplyToId);
	}

	public async Task<Post> Draft(Post post, Uuid7? inReplyToId = default, bool publish = false)
	{
		using var span = _instrument.Span<PostService>();

		if (inReplyToId is { } parentId)
		{
			var parent = await _posts.LookupPost(parentId)
						 ?? throw CoreException.MissingData($"Couldn't find post {parentId} to reply to", typeof(Post),
							 parentId);
			post.InReplyTo = parent;
			post.Thread = parent.Thread;
			post.Thread.Posts.Add(post);
		}

		if (await _posts.LookupProfile(_profileId) is not { } author)
			throw CoreException.MissingData<Profile>($"Couldn't find profile {_profileId}", _profileId);
		post.Creators.Clear();
		post.Creators.Add(author);
		foreach (var content in post.Contents)
		{
			content.Sanitize(_sanitizers);
		}

		if (post.Audience.Contains(Audience.Public))
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
			_posts.Update(audience);
		}

		if (publish) post.PublishedDate = DateTimeOffset.UtcNow;
		_posts.Add(post);
		await _posts.Commit();
		span?.AddTag("letterbook.post.audience", string.Join(",", post.Audience.Select(a => a.FediId)));
		span?.AddTag("letterbook.post.mentions", string.Join(",", post.AddressedTo.Select(m => m.Id)));
		await _postEventPublisher.Created(post, _profileId, _claims);
		if (publish) await _postEventPublisher.Published(post, _profileId, _claims);

		return post;
	}

	public async Task<Post> Update(Uuid7 postId, Post post)
	{
		// TODO: authz
		// I think authz can be conveyed around the app with just a list of claims, as long as one of the claims is
		// a profile, right?
		var previous = await _posts.LookupPost(postId)
					   ?? throw CoreException.MissingData($"Could not find existing post {post.Id} to update",
						   typeof(Post), postId);

		if (!(previous as IFederated).StrictEqual(post))
			throw CoreException.InvalidRequest("Input IDs don't match", $"{postId.ToId25String()}", post);
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


		_posts.Update(previous);
		await _posts.Commit();
		await _postEventPublisher.Updated(previous, _profileId, _claims);

		return previous;
	}

	public async Task Delete(Uuid7 id)
	{
		var post = await _posts.LookupPost(id);
		if (post is null) return;
		// TODO: authz and thread root
		post.DeletedDate = DateTimeOffset.UtcNow;
		_posts.Remove(post);
		await _posts.Commit();
		await _postEventPublisher.Deleted(post, _profileId, _claims);
	}

	public async Task<Post> Publish(Uuid7 id, bool localOnly = false)
	{
		var post = await _posts.LookupPost(id);
		if (post is null) throw CoreException.MissingData<Post>($"Can't find post {id} to publish", id);
		if (post.PublishedDate is not null)
			throw CoreException.Duplicate($"Tried to publish post {id} that is already published", id);
		post.PublishedDate = DateTimeOffset.UtcNow;
		post.CreatedDate = DateTimeOffset.UtcNow;

		_posts.Update(post);
		await _posts.Commit();
		await _postEventPublisher.Published(post, _profileId, _claims);
		return post;
	}

	public async Task<Post> AddContent(Uuid7 postId, Content content)
	{
		var post = await _posts.LookupPost(postId)
				   ?? throw CoreException.MissingData<Post>("Can't find existing post to add content", postId);
		if (!post.FediId.HasLocalAuthority(_options))
			throw CoreException.WrongAuthority("Can't modify contents of remote post", post.FediId);
		content.SortKey ??= (post.Contents.Select(c => c.SortKey).Max() ?? -1) + 1;
		content.Sanitize(_sanitizers);
		post.Contents.Add(content);

		if (post.PublishedDate is not null)
		{
			post.UpdatedDate = DateTimeOffset.UtcNow;
			await _postEventPublisher.Published(post, _profileId, _claims);
		}

		_posts.Update(post);
		await _posts.Commit();
		await _postEventPublisher.Updated(post, _profileId, _claims);
		return post;
	}

	public async Task<Post> RemoveContent(Uuid7 postId, Uuid7 contentId)
	{
		var post = await _posts.LookupPost(postId)
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
			await _postEventPublisher.Published(post, _profileId, _claims);
		}

		_posts.Remove(content);
		_posts.Update(post);
		await _posts.Commit();
		await _postEventPublisher.Updated(post, _profileId, _claims);
		return post;
	}

	public async Task<Post> UpdateContent(Uuid7 postId, Uuid7 contentId, Content content)
	{
		var post = await _posts.LookupPost(postId)
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
			await _postEventPublisher.Published(post, _profileId, _claims);
		}

		_posts.Update(post);
		await _posts.Commit();
		await _postEventPublisher.Updated(post, _profileId, _claims);
		return post;
	}

	public async Task<Post> ReceiveCreate(Post post)
	{
		throw new NotImplementedException();
	}

	public async Task<Post> ReceiveUpdate(Post post)
	{
		throw new NotImplementedException();
	}

	public async Task<Post> ReceiveUpdate(Uri post)
	{
		throw new NotImplementedException();
	}

	public async Task<Post> ReceiveDelete(Uri post)
	{
		throw new NotImplementedException();
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

	public async Task Share(Uuid7 id)
	{
		throw new NotImplementedException();
	}

	public async Task Like(Uuid7 id)
	{
		throw new NotImplementedException();
	}

	private async Task<Profile?> ResolveProfile(Uri profileId,
		Profile? onBehalfOf = null,
		[CallerMemberName] string name = "",
		[CallerFilePath] string path = "",
		[CallerLineNumber] int line = -1)
	{
		// TODO: Authz
		var profile = await _posts.LookupProfile(profileId);
		if (profile != null
			&& !profile.HasLocalAuthority(_options)
			&& profile.Updated.Add(TimeSpan.FromHours(12)) >= DateTime.UtcNow) return profile;

		try
		{
			if (profile != null)
			{
				var fetched = await _apClient.As(onBehalfOf).Fetch<Profile>(profileId);
				profile.ShallowCopy(fetched);
				_posts.Update(profile);
			}
			else
			{
				profile = await _apClient.As(onBehalfOf).Fetch<Profile>(profileId);
				_posts.Add(profile);
			}
			_logger.LogInformation("Fetched Profile {ProfileId} from origin", profileId);
		}
		catch (AdapterException ex)
		{
			_logger.LogError(ex, "Cannot resolve Profile {ProfileId}", profileId);
		}

		return profile;
	}

	private async Task<Post?> ResolvePost(Uri postId,
		Profile? onBehalfOf = null,
		[CallerMemberName] string name = "",
		[CallerFilePath] string path = "",
		[CallerLineNumber] int line = -1)
	{
		var knownPost = false;
		// TODO: Authz
		if (await _posts.LookupPost(postId) is { } post)
		{
			if (post.HasLocalAuthority(_options)) return post;
			if (post.LastSeenDate >= DateTimeOffset.UtcNow - TimeSpan.FromMinutes(10)) return post;
			knownPost = true;
		}

		if (postId.HasLocalAuthority(_options))
		{
			_logger.LogWarning("Tried to lookup local post {PostId} that doesn't exist", postId);
			return default;
		}

		try
		{
			post = await _apClient.As(onBehalfOf).Fetch<Post>(postId);
			if (knownPost) _posts.Update(post);
			else _posts.Add(post);
			return post;
		}
		catch (AdapterException e)
		{
			_logger.LogWarning(e, "Cannot resolve post {Post}", postId);
			return default;
		}
	}

	public IAuthzPostService As(IEnumerable<Claim> claims, Uuid7 profileId)
	{
		_profileId = profileId;
		_claims = claims;

		return this;
	}
}