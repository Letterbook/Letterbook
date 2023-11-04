using Letterbook.Core.Adapters;
using Letterbook.Core.Models;

namespace Letterbook.Core;

public class ProfileEventService : IProfileEventService
{
    public void Created(Profile profile)
    {
        throw new NotImplementedException();
    }

    public void Deleted(Profile profile)
    {
        throw new NotImplementedException();
    }

    public void Updated(Profile original, Profile updated)
    {
        throw new NotImplementedException();
    }

    public void MigratedIn(Profile profile)
    {
        throw new NotImplementedException();
    }

    public void MigratedOut(Profile profile)
    {
        throw new NotImplementedException();
    }

    public void Reported(Profile profile)
    {
        throw new NotImplementedException();
    }

    public void Blocked(Profile profile)
    {
        throw new NotImplementedException();
    }
}