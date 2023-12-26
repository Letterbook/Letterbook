using Letterbook.Core.Models;

namespace Letterbook.Core;

public interface IActivityService
{
    Task<bool> ReceiveNotes(IEnumerable<Note> notes, ActivityType activity, Profile actor);
}