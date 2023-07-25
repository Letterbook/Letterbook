using Letterbook.Core.Adapters;
using Letterbook.Core.Models;

namespace Letterbook.Core;

/// <summary>
/// ActivityEventService provides a loosely coupled publish/subscribe interface between other core services. Typically,
/// core services will be narrowly scoped, and will publish a message through the event service. Other services may
/// consume that message and perform their own processing in response. For instance, to send notifications.
/// </summary>
public class ActivityEventService : IActivityEventService
{
    private IMessageBusAdapter _messageBusAdapter;

    public ActivityEventService(IMessageBusAdapter messageBusAdapter)
    {
        _messageBusAdapter = messageBusAdapter;
    }

    public void Created<T>(T value) where T : class, IObjectRef
    {
        throw new NotImplementedException();
    }

    public void Updated<T>(T value) where T : class, IObjectRef
    {
        throw new NotImplementedException();
    }

    public void Deleted<T>(T value) where T : class, IObjectRef
    {
        throw new NotImplementedException();
    }

    public void Flagged<T>(T value) where T : class, IObjectRef
    {
        throw new NotImplementedException();
    }

    public void Liked<T>(T value) where T : class, IObjectRef
    {
        throw new NotImplementedException();
    }

    public void Boosted<T>(T value) where T : class, IObjectRef
    {
        throw new NotImplementedException();
    }

    public void Approved<T>(T value) where T : class, IObjectRef
    {
        throw new NotImplementedException();
    }

    public void Rejected<T>(T value) where T : class, IObjectRef
    {
        throw new NotImplementedException();
    }

    public void Requested<T>(T value) where T : class, IObjectRef
    {
        throw new NotImplementedException();
    }

    public void Mentioned<T>(T value) where T : class, IObjectRef
    {
        throw new NotImplementedException();
    }
}
