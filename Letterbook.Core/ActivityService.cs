using Letterbook.ActivityPub;
using Letterbook.Core.Adapters;
using Letterbook.Core.Extensions;
using Letterbook.ActivityPub.Models;
using Microsoft.Extensions.Logging;
using Object = Letterbook.ActivityPub.Models.Object;
using PubObject = Fedodo.NuGet.ActivityPub.Model.CoreTypes.Object;

namespace Letterbook.Core;

public class ActivityService : IActivityService
{
    private readonly IFediAdapter _fediAdapter;
    private readonly IActivityAdapter _activityAdapter;
    private readonly IShareAdapter _shareAdapter;
    private readonly ILogger<ActivityService> _logger;

    public ActivityService(IFediAdapter fediAdapter, IActivityAdapter activityAdapter, IShareAdapter shareAdapter,
        ILogger<ActivityService> logger)
    {
        _fediAdapter = fediAdapter;
        _activityAdapter = activityAdapter;
        _shareAdapter = shareAdapter;
        _logger = logger;
    }

    public Activity Create()
    {
        throw new NotImplementedException();
    }

    public async Task Receive(Activity activity)
    {
        // get destinations
        var recipients = activity.To
            .Concat(activity.Cc)
            .Concat(activity.Bto)
            .Concat(activity.Bcc);

        // get audience (in case there is one)
        // also includes recipients
        var audience = recipients.Concat(activity.Audience);

        // record activity
        // TODO: what about more than one object in an activity?
        // TODO: handle different kinds of activities
        // TODO: do all activities have an object? Either way, handle invalid inputs
        var subject = activity.Object.FirstOrDefault().Resolve();
        // await _activityAdapter.RecordObject(subject);

        // add to audience inboxes
        // var shareTasks = audience.Select(a => _shareAdapter.ShareWithAudience(subject, a.Url.ToString()));
        // await Task.WhenAll(shareTasks);

        // notify recipients
        // TODO: NotificationService
        // for now, just log it to prove we got it
        var resolved = activity.Object.FirstOrDefault().TryResolve<Object>(out var value);
        _logger.LogInformation("Activity received: {type} {object}", activity.Type,
            resolved ? value!.Type.First() : "Unknown");
    }

    public void Deliver(Activity activity)
    {
        throw new NotImplementedException();
    }

    private async Task<PubObject?> ResolveLink(Link link)
    {
        return link.Href != null
            ? await _fediAdapter.FollowLink(link.Href)
            : default;
    }

    private async Task<IEnumerable<Profile>> GetAudienceMembers(Collection collection)
    {
        throw new NotImplementedException();
    }
}