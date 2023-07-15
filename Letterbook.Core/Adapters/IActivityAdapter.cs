using Letterbook.Core.Models;

namespace Letterbook.Core.Adapters;

public interface IActivityAdapter : IAdapter
{
    ValueTask<bool> RecordNote(Note note);
}
