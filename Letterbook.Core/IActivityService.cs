using Letterbook.ActivityPub.Models;

namespace Letterbook.Core;

public interface IActivityService
{
    Activity Create();
    Task Receive(Activity activity);
    void Deliver(Activity activity);
}