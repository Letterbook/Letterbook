using Letterbook.Core.Models;

namespace Letterbook.Core.Adapters;

public interface IActivityAdapter
{
    ValueTask<bool> RecordNote(Note note);
}