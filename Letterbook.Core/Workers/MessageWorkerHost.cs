using ActivityPub.Types.AS;
using CloudNative.CloudEvents;
using Letterbook.Core.Adapters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Letterbook.Core.Workers;

/// <summary>
/// A hosted service for message observers that will manage the observer's subscription on a specific channel
/// </summary>
/// <typeparam name="TObserver">A class encapsulating the observer, which must manage a scope for each observation emitted by
/// the Observable</typeparam>
/// <typeparam name="TChannel">The Type of the channel to subscribe</typeparam>
public class MessageWorkerHost<TObserver, TChannel> : BackgroundService where TObserver : IMessageObserver
{
	private readonly ILogger<MessageWorkerHost<TObserver, TChannel>> _logger;
	private readonly IServiceProvider _provider;
	private readonly IMessageBusClient _messageBus;
	private readonly IObservable<CloudEvent> _observable;
	private IDisposable? _subscription;

	public MessageWorkerHost(ILogger<MessageWorkerHost<TObserver, TChannel>> logger, IServiceProvider provider, IMessageBusClient messageBus, int handlerDelay)
	{
		_logger = logger;
		_provider = provider;
		_messageBus = messageBus;
		// TODO(RabbitMQ): make this delay play nice
		_observable = _messageBus.ListenChannel<TChannel>(TimeSpan.FromMilliseconds(handlerDelay), typeof(TObserver).ToString());
	}

	protected override Task ExecuteAsync(CancellationToken stoppingToken)
	{
		var observer = _provider.GetRequiredService<TObserver>();
		observer.SetCancellationToken(stoppingToken);

		_subscription = _observable.Subscribe(observer);

		stoppingToken.Register(() =>
		{
			_subscription.Dispose();
		});
		return Task.CompletedTask;
	}
}

public interface IMessageObserver : IObserver<CloudEvent>
{
	public void SetCancellationToken(CancellationToken token);
}