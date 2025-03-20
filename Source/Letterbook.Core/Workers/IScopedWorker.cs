namespace Letterbook.Core.Workers;

public interface IScopedWorker
{
	Task DoWork(CancellationToken cancellationToken);
}