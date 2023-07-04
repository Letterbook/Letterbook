using Letterbook.Core.Adapters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Object = Fedodo.NuGet.ActivityPub.Model.CoreTypes.Object;

namespace Letterbook.Adapter.Db;

public class ActivityAdapter : IActivityAdapter, IShareAdapter
{
    private readonly ILogger<ActivityAdapter> _logger;
    private readonly DbOptions _config;

    public ActivityAdapter(ILogger<ActivityAdapter> logger, IOptions<DbOptions> config)
    {
        _logger = logger;
        _config = config.Value;
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