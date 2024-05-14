using CloudNative.CloudEvents;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Letterbook.Core.Workers;

/// <summary>
/// Observe events emitted by an Observable channel, and manage a scope for each event.
/// Work to handle the event is delegate to the IObserverWorker
/// </summary>
/// <typeparam name="T">The IObserverWorker that will process the event. It will have access to scoped services.</typeparam>
public class EventObserver<T> : IEventObserver<T> where T : IObserverWorker
{
	private CancellationToken? _cancellationToken;
	private ILogger<EventObserver<T>> _logger;
	private readonly IServiceProvider _provider;

	public EventObserver(ILogger<EventObserver<T>> logger, IServiceProvider provider)
	{
		_logger = logger;
		_provider = provider;
	}

	public void OnCompleted()
	{
		_logger.LogWarning("{Worker} channel closed", nameof(T));
	}

	public void OnError(Exception error)
	{
		_logger.LogError(error, "{Worker} channel error {Message}", nameof(T), error.Message);
	}

	public void OnNext(CloudEvent message)
	{
		using var scope = _provider.CreateScope();
		var worker = scope.ServiceProvider.GetRequiredService<T>();
		var task = worker.DoWork(message, GetCancelToken());

		try
		{
			task.Wait(GetCancelToken());
		}
		catch (Exception e)
		{
			_logger.LogError(e, "{Worker} handler exception {Message}", nameof(worker.GetType), e.Message);
		}
	}

	public void SetCancellationToken(CancellationToken token) => _cancellationToken = token;

	private CancellationToken GetCancelToken() => _cancellationToken ?? CancellationToken.None;
}