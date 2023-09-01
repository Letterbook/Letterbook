using Letterbook.Core.Adapters;
using Letterbook.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Letterbook.Adapter.Db;

public class ActivityAdapter : IActivityAdapter
{
    private readonly ILogger<ActivityAdapter> _logger;
    private readonly RelationalContext _context;

    public ActivityAdapter(ILogger<ActivityAdapter> logger, RelationalContext context)
    {
        _logger = logger;
        _context = context;
    }

    public bool RecordNote(Note obj)
    {
        _logger.LogInformation("Called {Name}", $"{nameof(RecordNote)}");
        var entity = _context.Notes.Add(obj);
        return entity.State is EntityState.Added or EntityState.Modified or EntityState.Unchanged;
    }

    public bool RecordNotes(IEnumerable<Note> notes)
    {
        _context.Notes.AddRange(notes);
        return true;
    }

    public bool DeleteNote(Note note)
    {
        throw new NotImplementedException();
    }

    public bool DeleteNotes(IEnumerable<Note> notes)
    {
        throw new NotImplementedException();
    }

    // TODO: Backing entities for revisions
    public bool AddRevision(Note note)
    {
        throw new NotImplementedException();
    }

    public Note? LookupNoteUrl(string url)
    {
        throw new NotImplementedException();
    }

    public Note? LookupNoteId(string? localId)
    {
        throw new NotImplementedException();
    }
}