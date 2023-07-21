using Letterbook.Core.Adapters;
using Letterbook.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Object = Fedodo.NuGet.ActivityPub.Model.CoreTypes.Object;

namespace Letterbook.Adapter.Db;

public class ActivityAdapter : IActivityAdapter, IShareAdapter
{
    private readonly ILogger<ActivityAdapter> _logger;
    private readonly TransactionalContext _context;

    public ActivityAdapter(ILogger<ActivityAdapter> logger, TransactionalContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async ValueTask<bool> RecordNote(Note obj)
    {
        _logger.LogInformation("Called {Name}", $"{nameof(RecordNote)}");
        var entity = await _context.Notes.AddAsync(obj);
        return entity.State is EntityState.Added or EntityState.Modified or EntityState.Unchanged;
    }

    public Task ShareWithAudience(Object obj, string audienceUri)
    {
        _logger.LogInformation("Called {Name}", $"{nameof(ShareWithAudience)}");
        return Task.CompletedTask;
    }
}