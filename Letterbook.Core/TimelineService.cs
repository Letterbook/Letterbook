using Letterbook.Core.Adapters;
using Letterbook.Core.Exceptions;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Letterbook.Core;

public class TimelineService : ITimelineService
{
	private ILogger<TimelineService> _logger;
	private CoreOptions _options;
	private IFeedsAdapter _feeds;
	private IAccountProfileAdapter _profileAdapter;

	public TimelineService(ILogger<TimelineService> logger, IOptions<CoreOptions> options, IFeedsAdapter feeds, IAccountProfileAdapter profileAdapter)
	{
		_logger = logger;
		_options = options.Value;
		_feeds = feeds;
		_profileAdapter = profileAdapter;
	}

	public void HandleCreate(Post post)
	{
		// TODO: account for moderation conditions (blocks, etc)
		var audience = DefaultAudience(post);
		var mentions = post.AddressedTo.Where(mention => mention.Subject.HasLocalAuthority(_options)).ToArray();

		audience.UnionWith(mentions.Select(mention => Audience.FromMention(mention.Subject)));
		_feeds.AddToTimeline(post);

		foreach (var mention in mentions)
		{
			_feeds.AddNotification(mention.Subject, post, ActivityType.Create);
		}
	}

	public void HandleBoost(Post post)
	{
		var boostedBy = post.SharesCollection.Last();
		if (post.Audience.Contains(Audience.Public)
			|| post.AddressedTo.Contains(Mention.Public)
			|| post.AddressedTo.Contains(Mention.Unlisted))
		{
			_feeds.AddToTimeline(post, boostedBy);
		}

		foreach (var creator in post.Creators.Where(creator => creator.HasLocalAuthority(_options)))
		{
			_feeds.AddNotification(creator, post, ActivityType.Announce, boostedBy);
		}
	}

	public void HandleUpdate(Post note)
	{
		var audience = DefaultAudience(note);
		var mentions = note.AddressedTo.Where(mention => mention.Subject.HasLocalAuthority(_options)).ToArray();

		audience.UnionWith(mentions.Select(mention => Audience.FromMention(mention.Subject)));
		_feeds.AddToTimeline(note);

		foreach (var mention in mentions)
		{
			_feeds.AddNotification(mention.Subject, note, ActivityType.Update);
		}

		foreach (var profile in note.SharesCollection.Where(profile => profile.HasLocalAuthority(_options)))
		{
			_feeds.AddNotification(profile, note, ActivityType.Update);
		}

		if (note.Creators.Count <= 1) return;
		foreach (var profile in note.Creators.Where(profile => profile.HasLocalAuthority(_options)))
		{
			_feeds.AddNotification(profile, note, ActivityType.Update);
		}
	}

	public void HandleDelete(Post note)
	{
		// TODO: Also handle deleted boosts
		_feeds.RemoveFromTimelines(note);
	}


	// public async Task<IEnumerable<Post>> GetFeed(Guid recipientId, DateTime begin, int limit = 40)
	public async Task<IEnumerable<TimelineEntry>> GetFeed(Guid recipientId, DateTime begin, int limit = 40)
	{
		// TODO(moderation): Account for moderation conditions (block, mute, etc)
		var recipient = await _profileAdapter.LookupProfile(recipientId);
		return recipient != null
			? _feeds.GetTimelineEntries(recipient.Audiences, begin, limit)
			: throw CoreException.MissingData("Couldn't lookup Profile to load Feed", typeof(Guid), recipientId);
	}

	/// <summary>
	/// Get the audience entries for the addressed recipients, plus followers/public/local, if applicable
	/// </summary>
	/// <param name="note"></param>
	/// <returns></returns>
	private HashSet<Audience> DefaultAudience(Post post)
	{
		var result = new HashSet<Audience>();
		result.UnionWith(post.Audience);

		// The "public audience" would be equivalent to Mastodon's federated global feed
		// That's not the same thing as putting posts into follower's feeds.
		// This ensures we include public posts in the followers audience in case the sender doesn't specify it
		if (!result.Contains(Audience.Public)) return result;
		result.UnionWith(post.Creators.Select(c => Audience.FromUri(c.Followers, c)));

		return result;
	}
}