using Letterbook.Core.Adapters;
using Letterbook.Core.Models;
using Microsoft.Extensions.Logging;

namespace Letterbook.Core;

/// <summary>
/// This whole class probably needs to be replaced. It just isn't going to handle activities this way.
/// </summary>
public class ActivityService : IActivityService
{
    private readonly IActivityAdapter _activityAdapter;
    private readonly ILogger<ActivityService> _logger;
    private readonly IActivityEventService _events;

    // TODO: remove/replace ActivityAdapter. We should map to and persist core data models
    public ActivityService(IActivityAdapter activityAdapter, ILogger<ActivityService> logger,
        IActivityEventService eventService)
    {
        _activityAdapter = activityAdapter;
        _logger = logger;
        _events = eventService;
    }

    // TODO(Rebuild): I don't know if this pattern will really work anymore
    // If nothing else, the AP types should really only be mapped in adapters. Core logic should only work on
    // core data models as much as possible.
    public async Task<bool> ReceiveNotes(IEnumerable<Note> notes, ActivityType activity, Profile actor)
    {
        var actionTaken = false;
        switch (activity)
        {
            case ActivityType.Create:
                actionTaken = _activityAdapter.RecordNotes(notes);
                if (!actionTaken) return actionTaken;
                foreach (var note in notes)
                {
                    _events.Created(note);
                }

                return actionTaken;
            case ActivityType.Announce:
                actionTaken = _activityAdapter.RecordNotes(notes);
                if (!actionTaken) return actionTaken;
                foreach (var note in notes)
                {
                    _events.Boosted(note);
                }

                return actionTaken;
            case ActivityType.Update:
                actionTaken = _activityAdapter.RecordNotes(notes);
                if (!actionTaken) return actionTaken;
                foreach (var note in notes)
                {
                    _events.Updated(note);
                }

                return actionTaken;
            case ActivityType.Like:
                foreach (var note in notes)
                {
                    var found = _activityAdapter.LookupNoteUrl(note.Id.ToString());
                    if (found is null) continue;
                    found.LikedBy.Add(actor);
                    _events.Liked(note);
                    actionTaken = true;
                }

                // publish like
                return actionTaken;
            case ActivityType.Delete:
                actionTaken = _activityAdapter.DeleteNotes(notes);
                if (!actionTaken) return actionTaken;
                foreach (var note in notes)
                {
                    _events.Deleted(note);
                }

                return actionTaken;
            case ActivityType.Dislike:
                actionTaken = notes
                    .Select(note => _activityAdapter.LookupNoteUrl(note.Id.ToString())?.LikedBy.Remove(actor))
                    .Any(r => r == true);
                return actionTaken;
            case ActivityType.Flag:
                // TODO: Flag
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
}