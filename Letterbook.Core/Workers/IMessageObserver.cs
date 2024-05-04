using CloudNative.CloudEvents;

namespace Letterbook.Core.Workers;

public interface IMessageObserver<T> : IObserver<CloudEvent> where T : IObserverWorker
{
	public void SetCancellationToken(CancellationToken token);
}