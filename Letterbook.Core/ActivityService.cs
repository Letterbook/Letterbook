using System.Runtime.InteropServices.JavaScript;
using Fedodo.NuGet.ActivityPub.Model.CoreTypes;
using Letterbook.Core.Ports;
using Microsoft.Extensions.Logging;
using PubObject = Fedodo.NuGet.ActivityPub.Model.CoreTypes.Object;

namespace Letterbook.Core;

public class ActivityService : IActivityService
{
    private readonly IFediPort _fediAdapter;
    private readonly IActivityPort _activityAdapter;
    private readonly ISharePort _shareAdapter;
    private readonly ILogger<ActivityService> _logger;

    public ActivityService(IFediPort fediAdapter, IActivityPort activityAdapter, ISharePort shareAdapter,
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
        var recipients = activity.To?.Objects ?? Array.Empty<PubObject>()
            .Concat(activity.Bto?.Objects ?? Array.Empty<PubObject>())
            .Concat(activity.Cc?.Objects ?? Array.Empty<PubObject>())
            .Concat(activity.Bcc?.Objects ?? Array.Empty<PubObject>());
        
        // get audience (often followers + public)
        // also includes recipients
        var audience = activity.Audience.Objects
            .Concat(recipients);

        // record activity
        // TODO: what about more than one object in an activity?
        // TODO: handle different kinds of activities
        var subject = activity.Object.Objects.First();
        await _activityAdapter.RecordObject(subject);
        
        // add to audience inboxes
        var shareTasks = audience.Select(a => _shareAdapter.ShareWithAudience(subject, a.Url.ToString()));
        await Task.WhenAll(shareTasks);
        
        // notify recipients
        // TODO: NotificationService
        // for now, just log it to prove we got it
        _logger.LogInformation("Activity received: {type} {object}", activity.Type,
            activity.Object?.Objects?.First().Type);
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