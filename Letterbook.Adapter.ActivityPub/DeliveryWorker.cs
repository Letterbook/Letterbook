using ActivityPub.Types.AS;
using CloudNative.CloudEvents;
using Letterbook.Core.Adapters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Letterbook.Adapter.ActivityPub;

public class DeliveryWorker : IObserver<CloudEvent>
{
    private readonly ILogger<DeliveryWorker> _logger;
    private readonly IServiceProvider _provider;

    public DeliveryWorker(ILogger<DeliveryWorker> logger, IServiceProvider provider)
    {
        _logger = logger;
        _provider = provider;
    }

    public void OnCompleted()
    {
        _logger.LogWarning("{Worker} completed", nameof(DeliveryWorker));
        throw new NotImplementedException();
    }

    public void OnError(Exception error)
    {
        _logger.LogError("{Worker} errored", nameof(DeliveryWorker));
        throw new NotImplementedException();
    }

    public async void OnNext(CloudEvent value)
    {
        _logger.LogDebug("Handle message {Type}", value.Type);
        var scope = _provider.CreateScope();
        if (value[IActivityMessageService.ProfileKey] is not Models.Profile profile ||
            value.Data is not ASType document ||
            value[IActivityMessageService.DestinationKey] is not Uri destination)
            return; // TODO: HERE

        var client = scope.ServiceProvider.GetRequiredService<IActivityPubClient>().As(profile);
        var response = await client.SendDocument(destination, document);
        _logger.LogDebug("Handled message {Type}", value.Type);
    }
}