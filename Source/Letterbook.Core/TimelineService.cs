using System.Security.Claims;
using Letterbook.Core.Adapters;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models;
using Letterbook.Core.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Letterbook.Core;

public class TimelineService : IAuthzTimelineService, ITimelineService
{
	private ILogger<TimelineService> _logger;
	private CoreOptions _options;
	private IFeedsAdapter _feeds;
	private IDataAdapter _data;
	private readonly IAuthorizationService _authz;
	private IEnumerable<Claim>? _claims;

	public TimelineService(ILogger<TimelineService> logger, IOptions<CoreOptions> options, IFeedsAdapter feeds, IDataAdapter data, IAuthorizationService authz)
	{
		_logger = logger;
		_options = options.Value;
		_feeds = feeds;
		_data = data;
		_authz = authz;
	}

	/// <inheritdoc />
	public async Task HandlePublish(Post post)
	{
		var profiles = await _data.Profiles([..post.AddressedTo.Select(m => m.Subject.Id)])
			.ToDictionaryAsync(p => p.Id);
		post.ConvergeMentions(profiles);
		// TODO: account for moderation conditions (blocks, etc)
		post.Audience = NormalizeAudience(post);
		_feeds.AddToTimeline(post);
		await _feeds.Commit();
	}

	/// <inheritdoc />
	public async Task HandleShare(Post post, Profile sharedBy)
	{
		var boostedBy = post.SharesCollection.Last();
		_feeds.AddToTimeline(post, boostedBy);
		await _feeds.Commit();
	}

	/// <inheritdoc />
	public async Task HandleUpdate(Post post, Post oldPost)
	{
		var removed = NormalizeAudience(oldPost);
		removed.ExceptWith(NormalizeAudience(post));
		var added = NormalizeAudience(post);
		added.ExceptWith(NormalizeAudience(oldPost));

		await _feeds.Start();
		if (added.Count != 0)
		{
			post.Audience = added;
			_feeds.AddToTimeline(post);
		}

		if (removed.Count != 0) await _feeds.RemoveFromTimelines(post, removed);

		if (post.Preview != oldPost.Preview) await _feeds.UpdateTimeline(post);
		await _feeds.Commit();
	}

	/// <inheritdoc />
	public async Task HandleDelete(Post note)
	{
		// TODO: Also handle deleted boosts
		await _feeds.RemoveFromAllTimelines(note);
	}


	/// <inheritdoc />
	public async Task<IEnumerable<Post>> GetFeed(ProfileId profileId, DateTimeOffset begin, int limit = 40)
	{
		// TODO(moderation): Account for moderation conditions (block, mute, etc)
		var audiences = await _data.Profiles(profileId).Include(p => p.Audiences).SelectMany(p => p.Audiences).ToListAsync();
		if (audiences.Count == 0)
		{
			_logger.LogWarning("Can't get feed for profile {ProfileId} because they are not a member of any audience", profileId);
			return [];
		}

		_logger.LogDebug("Getting feed for {Profile} with membership in {Count} Audiences", profileId, audiences.Count);
		var ids = await _feeds.GetTimelineEntries(audiences, begin, limit)
			.ToArrayAsync();
		_logger.LogDebug("Loaded {Count} timeline posts for {ProfileId}", ids.Length, profileId);

		var results = await _data.Posts(ids.Select(p => p.Id).ToArray()).WithTimelineFields().TagWithCallSite().ToDictionaryAsync(p => p.Id);
		return ids.Select(id =>
		{
			results.TryGetValue(id.Id, out var post);
			return post;
		}).WhereNotNull().ToList();
	}

	/// <summary>
	/// Get the audience entries for the addressed recipients, plus followers/public/local, if applicable
	/// </summary>
	/// <param name="post"></param>
	/// <returns></returns>
	private HashSet<Audience> NormalizeAudience(Post post)
	{
		var result = new HashSet<Audience>();
		result.UnionWith(post.Audience);
		result.UnionWith(post.AddressedTo.Where(
			m => m.Subject.HasLocalAuthority(_options)).Select(m => Audience.FromMention(m.Subject)));

		return result;
	}

	public IAuthzTimelineService As(IEnumerable<Claim> claims)
	{
		_claims = claims;
		return this;
	}
}