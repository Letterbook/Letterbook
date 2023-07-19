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
    public async Task<bool> ReceiveNotes(IEnumerable<Note> notes, ActivityType activity, Models.Profile actor)
    {
        var saved = false;
        switch (activity)
        {
            case ActivityType.Create:
                // 1. record those objects ✅
                saved = _activityAdapter.RecordNotes(notes);
                // 2. publish Notes (mostly not other types) to the queue
                return saved;
            case ActivityType.Like:
                foreach (var note in notes)
                {
                    note.LikedBy.Add(actor);
                }
                saved = _activityAdapter.RecordNotes(notes);
                // publish like
                return saved;
            case ActivityType.Announce:
            case ActivityType.Delete:
            case ActivityType.Dislike:
            case ActivityType.Flag:
            case ActivityType.Ignore: //?
            case ActivityType.Question: //?
            case ActivityType.Remove: //?
            case ActivityType.Undo: //?
            case ActivityType.Update:
                _logger.LogInformation("Ignored unimplemented Activity type {Activity}", activity);
                return false;
            case ActivityType.Unknown:
                _logger.LogInformation("Ignored unknown Activity type {Activity}", activity);
                return false;
            default:
                _logger.LogInformation("Ignored semantically nonsensical Activity {Activity}", activity);
                return false;
        }

        return true;
    }

    public void Deliver(Activity activity)
    {
        throw new NotImplementedException();
    }
}