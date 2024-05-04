using CloudNative.CloudEvents;

namespace Letterbook.Core.Workers;

public interface IEventObserver<T> : IObserver<CloudEvent> where T : IObserverWorker
{
	public void SetCancellationToken(CancellationToken token);
}