using CloudNative.CloudEvents;

namespace Letterbook.Core.Workers;

public interface IMessageObserver<T> : IObserver<CloudEvent> where T : IObserverWorker
{
	public void SetCancellationToken(CancellationToken token);
}

public interface IObserverWorker
{
	public Task DoWork(CloudEvent message, CancellationToken token);
}