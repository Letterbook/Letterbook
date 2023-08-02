using CloudNative.CloudEvents;

namespace Letterbook.Core.Adapters;

public interface IMessageBusAdapter
{
    public IObserver<CloudEvent> OpenChannel<T>();
}