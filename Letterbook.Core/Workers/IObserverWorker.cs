using CloudNative.CloudEvents;

namespace Letterbook.Core.Workers;

public interface IObserverWorker
{
	public Task DoWork(CloudEvent message, CancellationToken token);
}