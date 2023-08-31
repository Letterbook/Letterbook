using Letterbook.Core.Adapters;
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

    public void HandleCreate(Note note)
    {
        // TODO: account for moderation conditions (blocks, etc)
        var audience = DefaultAudience(note);
        var mentions = note.Mentions.Where(mention => mention.Subject.HasLocalAuthority(_options)).ToArray();

        audience.UnionWith(mentions.Select(mention => Audience.FromMention(mention.Subject)));
        _feeds.AddToTimeline(note, audience);

        foreach (var mention in mentions)
        {
            _feeds.AddNotification(mention.Subject, note, note.Creators, ActivityType.Create);
        }
    }

    public void HandleBoost(Note note)
    {
        var boostedBy = note.BoostedBy.Last();
        if (note.Visibility.Contains(Audience.Public) || note.Mentions.Contains(Mention.Public) ||
            note.Mentions.Contains(Mention.Unlisted))
        {
            _feeds.AddToTimeline(note, Audience.FromBoost(boostedBy), boostedBy);
        }

        foreach (var creator in note.Creators.Where(creator => creator.HasLocalAuthority(_options)))
        {
            _feeds.AddNotification(creator, note, new []{boostedBy}, ActivityType.Announce);
        }
        
    }

    public void HandleUpdate(Note note)
    {
        var audience = DefaultAudience(note);
        var mentions = note.Mentions.Where(mention => mention.Subject.HasLocalAuthority(_options)).ToArray();

        audience.UnionWith(mentions.Select(mention => Audience.FromMention(mention.Subject)));
        _feeds.AddToTimeline(note, audience);

        foreach (var mention in mentions)
        {
            _feeds.AddNotification(mention.Subject, note, note.Creators, ActivityType.Update);
        }

        foreach (var profile in note.BoostedBy.Where(profile => profile.HasLocalAuthority(_options)))
        {
            _feeds.AddNotification(profile, note, note.Creators, ActivityType.Update);
        }

        if (note.Creators.Count <= 1) return;
        foreach (var profile in note.Creators.Where(profile => profile.HasLocalAuthority(_options)))
        {
            _feeds.AddNotification(profile, note, note.Creators, ActivityType.Update);
        }
    }

    public void HandleDelete(Note note)
    {
        // Need to figure out how to handle deleted boosts
        _feeds.RemoveFromTimelines(note);
    }

    public IEnumerable<TimelineEntry> GetFeed(string recipientId, DateTime begin, int limit = 40)
    {
        // TODO: Account for moderation conditions (block, mute, etc)
        var recipient = _profileAdapter.LookupProfile(recipientId);
        return _feeds.GetTimelineEntries(recipient.Audiences, begin, limit);
    }

    /// <summary>
    /// Get the audience entries for the addressed recipients, plus followers/public/local, if applicable
    /// </summary>
    /// <param name="note"></param>
    /// <returns></returns>
    private HashSet<Audience> DefaultAudience(Note note)
    {
        var result = new HashSet<Audience>();
        result.UnionWith(note.Visibility);

        // The "public audience" would be equivalent to Mastodon's federated global feed
        // That's not the same thing as putting posts into follower's feeds.
        // This ensures we include public posts in the followers audience in case the sender doesn't specify it
        if (!result.Contains(Audience.Public)) return result;
        var followers = note.Creators.Select(p => p.FollowersCollection.Id);
        result.UnionWith(followers.Select(Audience.FromUri));

        return result;
    }
}