using System.Runtime.InteropServices.JavaScript;
using Fedodo.NuGet.ActivityPub.Model.CoreTypes;
using Letterbook.Core.Ports;
using PubObject = Fedodo.NuGet.ActivityPub.Model.CoreTypes.Object;

namespace Letterbook.Core;

public class ActivityService
{
    private readonly IFediPort _fediPort;
    private readonly IActivityPort _activityPort;
    private readonly ISharePort _sharePort;

    public ActivityService(IFediPort fediPort, IActivityPort activityPort, ISharePort sharePort)
    {
        _fediPort = fediPort;
        _activityPort = activityPort;
        _sharePort = sharePort;
    }

    public Activity Create()
    {
        throw new NotImplementedException();
    }

    public async Task Receive(Activity activity)
    {
        // get destinations
        var linkTasks = (activity.To?.Links?.Select(ResolveLink) ?? Array.Empty<Task<PubObject>>())
            .Concat(activity.Bto?.Links?.Select(ResolveLink) ?? Array.Empty<Task<PubObject>>())
            .Concat(activity.Cc?.Links?.Select(ResolveLink) ?? Array.Empty<Task<PubObject>>())
            .Concat(activity.Bcc?.Links?.Select(ResolveLink) ?? Array.Empty<Task<PubObject>>());
        var recipients = activity.To?.Objects ?? Array.Empty<PubObject>()
            .Concat(activity.Bto?.Objects ?? Array.Empty<PubObject>())
            .Concat(activity.Cc?.Objects ?? Array.Empty<PubObject>())
            .Concat(activity.Bcc?.Objects ?? Array.Empty<PubObject>());
        var linkedRecipients = await Task.WhenAll(linkTasks);
        
        // get audience (often followers + public)
        // also includes recipients
        var audience = activity.Audience.Objects;
        var linkedAudience = Task.WhenAll(activity.Audience.Links.Select(ResolveLink));

        // record activity
        await _activityPort.RecordObject(activity.Object.Objects.First());
        
        // add to audience inboxes
        var shareTasks = audience.Select(a => _sharePort.ShareWithAudience(activity, a.Url.ToString()));
        await Task.WhenAll(shareTasks);
        
        // notify recipients


        throw new NotImplementedException();
    }

    public void Deliver(Activity activity)
    {
        throw new NotImplementedException();
    }

    private async Task<PubObject?> ResolveLink(Link link)
    {
        return link.Href != null
            ? await _fediPort.FollowLink(link.Href)
            : default;
    }

    private async Task<IEnumerable<Profile>> GetAudienceMembers(Collection collection)
    {
        throw new NotImplementedException();
    }
}