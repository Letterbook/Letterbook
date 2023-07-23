using Letterbook.Core.Models;

namespace Letterbook.Core;

// TODO: Figure out how to make this work
public class ObjectEventService : IObjectEventService
{
    public IObservable<IObjectRef> ObjectEvents { get; }

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