using Letterbook.Core.Models;

namespace Letterbook.Core.Adapters;

public interface IActivityAdapter
{
    bool RecordNote(Note note);
    bool RecordNotes(IEnumerable<Note> notes);
}