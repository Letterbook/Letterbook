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
        throw new NotImplementedException();
    }

    public void OnError(Exception error)
    {
        throw new NotImplementedException();
    }

    public async void OnNext(CloudEvent value)
    {
        var scope = _provider.CreateScope();
        if (value[IActivityMessageService.ProfileKey] is not Models.Profile profile ||
            value.Data is not ASType document ||
            value[IActivityMessageService.DestinationKey] is not Uri destination)
            return;

        var client = scope.ServiceProvider.GetRequiredService<IActivityPubClient>().As(profile);
        await client.SendDocument(destination, document);
    }
}