using Letterbook.Core.Models;

namespace Letterbook.Core;

public interface IActivityEventService
{
    void Created<T>(T value) where T : class, IObjectRef;
    void Updated<T>(T value) where T : class, IObjectRef;
    void Deleted<T>(T value) where T : class, IObjectRef;
    void Flagged<T>(T value) where T : class, IObjectRef;
    void Liked<T>(T value) where T : class, IObjectRef;
    void Boosted<T>(T value) where T : class, IObjectRef;
    void Approved<T>(T value) where T : class, IObjectRef;
    void Rejected<T>(T value) where T : class, IObjectRef;
    void Requested<T>(T value) where T : class, IObjectRef;
    void Offered<T>(T value) where T : class, IObjectRef;
    void Mentioned<T>(T value) where T : class, IObjectRef;
}