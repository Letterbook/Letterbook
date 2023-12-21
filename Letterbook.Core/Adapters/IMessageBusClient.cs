using System.Runtime.CompilerServices;
using CloudNative.CloudEvents;

namespace Letterbook.Core.Adapters;

public interface IMessageBusClient
{
    public IObservable<CloudEvent> ListenChannel<T>([CallerMemberName] string? name = "");
}