using Letterbook.Core.Models;

namespace Letterbook.Core;

public interface IActivityService
{
    DTO.Activity Create();
    Task<bool> ReceiveNotes(IEnumerable<Note> notes, ActivityType activity, Profile actor);
    void Deliver(DTO.Activity activity);
}