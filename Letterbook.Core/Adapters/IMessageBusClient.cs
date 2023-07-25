namespace Letterbook.Core.Adapters;

public interface IMessageBusClient
{
    public IObservable<T> ListenChannel<T>();
}