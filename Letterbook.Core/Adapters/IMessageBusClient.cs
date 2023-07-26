using CloudNative.CloudEvents;

namespace Letterbook.Core.Adapters;

public interface IMessageBusClient
{
    public IObservable<CloudEvent> ListenChannel(string type);
}