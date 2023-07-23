using Letterbook.Core.Adapters;
using Letterbook.ActivityPub.Models;
using Letterbook.Core.Models;
using Microsoft.Extensions.Logging;

namespace Letterbook.Core;

public class ActivityService : IActivityService
{
    private readonly IFediAdapter _fediAdapter;
    private readonly IActivityAdapter _activityAdapter;
    private readonly ILogger<ActivityService> _logger;

    // TODO: pubsub adapter
    public ActivityService(IFediAdapter fediAdapter, IActivityAdapter activityAdapter, ILogger<ActivityService> logger)
    {
        _fediAdapter = fediAdapter;
        _activityAdapter = activityAdapter;
        _logger = logger;
    }

    public Activity Create()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="notes">The Notes references as the object of the activity</param>
    /// <param name="activity">The type of activity being taken</param>
    /// <param name="actor">The Actor to whom this activity is attributed</param>
    /// <returns>True if any action was taken, false otherwise</returns>
    public async Task<bool> ReceiveNotes(IEnumerable<Note> notes, ActivityType activity, Profile actor)
    {
        var actionTaken = false;
        switch (activity)
        {
            case ActivityType.Create:
            case ActivityType.Announce:
            case ActivityType.Update:
                // 1. record those objects ✅
                actionTaken = _activityAdapter.RecordNotes(notes);
                // 2. publish Notes (mostly not other types) to the queue
                return actionTaken;
            case ActivityType.Like:
                foreach (var note in notes)
                {
                    var found = _activityAdapter.LookupNoteUrl(note.Id.ToString());
                    if (found is null) continue;
                    found.LikedBy.Add(actor);
                    actionTaken = true;
                }
                // publish like
                return actionTaken;
            case ActivityType.Delete:
                actionTaken = _activityAdapter.DeleteNotes(notes);
                return actionTaken;
            case ActivityType.Dislike:
                actionTaken = notes
                    .Select(note => _activityAdapter.LookupNoteUrl(note.Id.ToString())?.LikedBy.Remove(actor))
                    .Any(r => r == true);
                return actionTaken;
            case ActivityType.Flag:
            case ActivityType.Ignore: //?
            case ActivityType.Question: //?
            case ActivityType.Remove: //?
            case ActivityType.Undo: //?
                _logger.LogInformation("Ignored unimplemented Activity type {Activity}", activity);
                return false;
            case ActivityType.Unknown:
                _logger.LogInformation("Ignored unknown Activity type {Activity}", activity);
                return false;
            default:
                _logger.LogInformation("Ignored semantically nonsensical Activity {Activity}", activity);
                return false;
        }
    }

    public void Deliver(Activity activity)
    {
        throw new NotImplementedException();
    }
}