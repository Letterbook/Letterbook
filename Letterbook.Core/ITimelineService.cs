using Letterbook.Core.Models;

namespace Letterbook.Core;

public interface ITimelineService
{
    public void HandleCreate(Note note);
    public void HandleBoost(Note note);
    public void HandleUpdate(Note note);
    public void HandleDelete(Note note);
}