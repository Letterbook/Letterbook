using Letterbook.Core.Models;

namespace Letterbook.Core.Adapters;

public interface IProfileEventService
{
    public void Created(Profile profile);
    public void Deleted(Profile profile);
    public void Updated(Profile profile);
    public void MigratedIn(Profile profile);
    public void MigratedOut(Profile profile);
    public void Reported(Profile profile);
    public void Blocked(Profile profile); // ?
}