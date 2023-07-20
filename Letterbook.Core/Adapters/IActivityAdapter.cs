using Letterbook.Core.Models;

namespace Letterbook.Core.Adapters;

public interface IActivityAdapter
{
    bool RecordNote(Note note);
    bool RecordNotes(IEnumerable<Note> notes);
    bool DeleteNote(Note note);
    bool DeleteNotes(IEnumerable<Note> notes);
    bool AddRevision(Note note);
    Note? LookupNoteUrl(string url);
    Note? LookupNoteId(string? localId);
    // Need to figure out more sophisticated queries
}