using CloudNative.CloudEvents;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Letterbook.Core.Workers;

public class DeliveryObserver : IMessageObserver
{
	private readonly ILogger<DeliveryObserver> _logger;
	private readonly IServiceProvider _provider;
	private CancellationToken? _cancellationToken;

	public DeliveryObserver(ILogger<DeliveryObserver> logger, IServiceProvider provider)
	{
		_logger = logger;
		_provider = provider;
	}

	public void OnCompleted()
	{
		_logger.LogWarning("{Worker} channel closed", nameof(DeliveryObserver));
	}

	public void OnError(Exception error)
	{
		_logger.LogError(error, "{Worker} channel error {Message}", nameof(DeliveryObserver), error.Message);
	}

	public void OnNext(CloudEvent message)
	{
		using var scope = _provider.CreateScope();
		var worker = scope.ServiceProvider.GetRequiredService<DeliveryWorker>();
		var task = worker.DoWork(message);

		try
		{
			if (_cancellationToken.HasValue)
			{
				task.Wait(_cancellationToken.Value);
			}
			else
			{
				task.Wait();
			}
		}
		catch (Exception e)
		{
			_logger.LogError(e, "{Worker} handler exception {Message}", nameof(worker.GetType), e.Message);
		}
	}

	public void SetCancellationToken(CancellationToken token) => _cancellationToken = token;
}