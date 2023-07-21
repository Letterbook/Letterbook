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
    // private static IMapper DtoMapper => new Mapper(Mappers.DtoMapper.Config);

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
                // 1. map the new objects
                // var objects = DtoMapper.Map<IEnumerable<IObjectRef>>(activity);
                // 2. record those objects ✅
                var recordResults = new List<bool>();
                // foreach (var objectRef in objects)
                // {
                    // if (objectRef is Note note)
                    // {
                        // recordResults.Add(await _activityAdapter.RecordNote(note));
                    // }
                // }
                
                if (!recordResults.Any(r => r)) return false;
                // 3. publish Notes (mostly not other types) to the queue
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

        return true;
    }

    public void Deliver(Activity activity)
    {
        throw new NotImplementedException();
    }
}