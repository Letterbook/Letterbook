using Letterbook.Core.Adapters;
using Letterbook.Core.Models;
using Microsoft.Extensions.Logging;

namespace Letterbook.Core;

public class TimelineService : ITimelineService
{
    private ILogger<TimelineService> _logger;
    private IFeedsAdapter _feeds;
    
    public TimelineService(ILogger<TimelineService> logger, IFeedsAdapter feeds)
    {
        _logger = logger;
        _feeds = feeds;
    }
    
    public void HandleCreate(Note note)
    {
        throw new NotImplementedException();
    }

    public void HandleBoost(Note note)
    {
        var boostedBy = note.BoostedBy.Last();
        throw new NotImplementedException();
    }

    public void HandleUpdate(Note note)
    {
        throw new NotImplementedException();
    }

    public void HandleDelete(Note note)
    {
        throw new NotImplementedException();
    }
}