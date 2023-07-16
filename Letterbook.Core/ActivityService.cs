using CommonExtensions;
using Letterbook.ActivityPub;
using Letterbook.Core.Adapters;
using Letterbook.Core.Extensions;
using Letterbook.ActivityPub.Models;
using Letterbook.Core.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="activity"></param>
    /// <returns>Success</returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<bool> Receive(Activity activity)
    {
        Enum.TryParse(activity.Type, out ActivityType type);
        switch (type)
        {
            case ActivityType.Create:
                return await HandleCreate(activity);
            case ActivityType.Like:
                throw new NotImplementedException("Not implemented yet: Like");
            default:
                _logger.LogInformation("Ignored unknown Activity Type {Type}", activity.Type);
                return false;
        }
        
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
        var resolved = activity.Object.FirstOrDefault().TryResolve<DTO.Object>(out var value);
        _logger.LogInformation("Activity received: {type} {object}", activity.Type,
            resolved ? value!.Type.First() : "Unknown");
    }

    public void Deliver(Activity activity)
    {
        throw new NotImplementedException();
    }

    private async Task<bool> HandleCreate(Activity activity)
    {
        var notes = activity.Object.Where(each => each.Type == "Note")
            .Where(each => each.Type == "Note")
            .Select(async each => await _activityAdapter.RecordNote(/*each as Note*/ new Note()))
            
    }
}