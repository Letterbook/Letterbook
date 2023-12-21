using ActivityPub.Types.AS;
using CloudNative.CloudEvents;
using Letterbook.Core.Adapters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Letterbook.Adapter.ActivityPub;

public class DeliveryWorkerHost : BackgroundService
{
    private readonly ILogger<DeliveryWorkerHost> _logger;
    private readonly IServiceProvider _provider;
    private readonly IMessageBusClient _messageBus;
    private readonly IObservable<CloudEvent> _observable;
    private IDisposable? _subscription;

    public DeliveryWorkerHost(ILogger<DeliveryWorkerHost> logger, IServiceProvider provider, IMessageBusClient messageBus)
    {
        _logger = logger;
        _provider = provider;
        _messageBus = messageBus;
        // This is... pretty much just a gross hack to give peers time to actually process their own stuff
        _observable = _messageBus.ListenChannel<ASType>(nameof(DeliveryWorkerHost));
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _subscription = _observable.Subscribe(_provider.GetRequiredService<DeliveryWorker>());
        stoppingToken.Register(() =>
        {
            _subscription.Dispose();
        });
        return Task.CompletedTask;
    }
}