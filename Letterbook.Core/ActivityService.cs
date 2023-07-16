using AutoMapper;
using Letterbook.ActivityPub;
using Letterbook.Core.Adapters;
using Letterbook.ActivityPub.Models;
using Letterbook.Core.Models;
using Microsoft.Extensions.Logging;
using PubObject = Fedodo.NuGet.ActivityPub.Model.CoreTypes.Object;

namespace Letterbook.Core;

public class ActivityService : IActivityService
{
    private readonly IFediAdapter _fediAdapter;
    private readonly IActivityAdapter _activityAdapter;
    private readonly IShareAdapter _shareAdapter;
    private readonly ILogger<ActivityService> _logger;
    private static IMapper DtoMapper => new Mapper(Mappers.DtoMapper.Config);

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
    public async Task<bool> Receive(DTO.Activity activity)
    {
        Enum.TryParse(activity.Type, out ActivityType type);
        switch (type)
        {
            case ActivityType.Create:
                // 1. look up the actors involved, create if we don't know them
                // 2. map the new objects
                // 3. record those objects
                // 4. publish Notes (mostly not other types) to the queue
                var objects = DtoMapper.Map<IEnumerable<IObjectRef>>(activity);
                var recordResults = new List<bool>();
                foreach (var objectRef in objects)
                {
                    if (objectRef is Note note)
                    {
                        recordResults.Add(await _activityAdapter.RecordNote(note));
                    }
                }
                
                if (!recordResults.Any(r => r)) return false;
                break;
            case ActivityType.Like:
                throw new NotImplementedException("Not implemented yet: Like");
            case ActivityType.Accept:
            case ActivityType.Add:
            case ActivityType.Announce:
            case ActivityType.Arrive:
            case ActivityType.Block:
            case ActivityType.Delete:
            case ActivityType.Dislike:
            case ActivityType.Flag:
            case ActivityType.Follow:
            case ActivityType.Ignore:
            case ActivityType.Invite:
            case ActivityType.Join:
            case ActivityType.Leave:
            case ActivityType.Listen:
            case ActivityType.Move:
            case ActivityType.Offer:
            case ActivityType.Question:
            case ActivityType.Reject:
            case ActivityType.Read:
            case ActivityType.Remove:
            case ActivityType.TentativeReject:
            case ActivityType.TentativeAccept:
            case ActivityType.Travel:
            case ActivityType.Undo:
            case ActivityType.Update:
            case ActivityType.View:
                _logger.LogInformation("Ignored unimplemented Activity type {Type}", type.ToString());
                return false;
            case ActivityType.Unknown:
            default:
                _logger.LogInformation("Ignored unknown Activity type {Type}", activity.Type);
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

    private async Task<bool> CreateNote(Note note)
    {
        var notes = note.Object.Where(each => each.Type == "Note")
            .Where(each => each.Type == "Note")
            .Select(async each => await _activityAdapter.RecordNote(/*each as Note*/ new Note()))
            
    }
}