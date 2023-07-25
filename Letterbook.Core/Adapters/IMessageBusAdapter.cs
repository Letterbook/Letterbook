namespace Letterbook.Core.Adapters;

public interface IMessageBusAdapter
{
    public IObserver<T> OpenChannel<T>();
}