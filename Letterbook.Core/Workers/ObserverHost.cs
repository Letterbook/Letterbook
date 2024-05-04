using CloudNative.CloudEvents;
using Letterbook.Core.Adapters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Letterbook.Core.Workers;

/// <summary>
/// A hosted service for message observers that will manage the Observer's subscription on a specific channel
/// </summary>
/// <typeparam name="TWorker">A class that will process Observable messages recieved by an Observer</typeparam>
/// <typeparam name="TChannel">The Type of the channel to subscribe</typeparam>
public class ObserverHost<TChannel, TWorker> : BackgroundService
	where TChannel : IEventType
	where TWorker : IObserverWorker
{
	private readonly IServiceProvider _provider;
	private readonly IMessageBusClient _messageBus;
	private readonly IObservable<CloudEvent> _observable;
	private IDisposable? _subscription;

	public ObserverHost(IServiceProvider provider, IMessageBusClient messageBus, int handlerDelay = 0)
	{
		_provider = provider;
		_messageBus = messageBus;
		// TODO(RabbitMQ): make this delay play nice
		_observable = _messageBus.ListenChannel<TChannel>(TimeSpan.FromMilliseconds(handlerDelay), typeof(TWorker).ToString());
	}

	protected override Task ExecuteAsync(CancellationToken stoppingToken)
	{
		var observer = _provider.GetRequiredService<IMessageObserver<TWorker>>();
		observer.SetCancellationToken(stoppingToken);

		_subscription = _observable.Subscribe(observer);

		stoppingToken.Register(() =>
		{
			_subscription.Dispose();
		});
		return Task.CompletedTask;
	}
}