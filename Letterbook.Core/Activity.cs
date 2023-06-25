using ApActivity = Fedodo.NuGet.ActivityPub.Model.CoreTypes.Activity;
namespace Letterbook.Core;

public class Activity
{
    public ApActivity Create()
    {
        throw new NotImplementedException();
    }

    public void Receive(ApActivity activity)
    {
        throw new NotImplementedException();
    }

    public void Deliver(ApActivity activity)
    {
        throw new NotImplementedException();
    }
}