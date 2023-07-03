using Letterbook.Core.Ports;
using Microsoft.Extensions.Logging;
using Object = Fedodo.NuGet.ActivityPub.Model.CoreTypes.Object;

namespace Letterbook.Adapter.Db;

public class ActivityAdapter : IActivityAdapter, IShareAdapter
{
    private readonly ILogger<ActivityAdapter> _logger;

    public ActivityAdapter(ILogger<ActivityAdapter> logger)
    {
        _logger = logger;
    }

    public Task RecordObject(Object obj)
    {
        _logger.LogInformation("Called {Name}", $"{nameof(RecordObject)}");
        return Task.CompletedTask;
    }

    public Task ShareWithAudience(Object obj, string audienceUri)
    {
        _logger.LogInformation("Called {Name}", $"{nameof(ShareWithAudience)}");
        return Task.CompletedTask;
    }
}