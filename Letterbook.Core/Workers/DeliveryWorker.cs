using CloudNative.CloudEvents;
using Letterbook.Core.Adapters;
using Medo;
using Microsoft.Extensions.Logging;

namespace Letterbook.Core.Workers;

public class DeliveryWorker
{
    private readonly ILogger<DeliveryWorker> _logger;
    private readonly IAccountProfileAdapter _profiles;
    private readonly IActivityPubClient _client;

    public DeliveryWorker(ILogger<DeliveryWorker> logger, IAccountProfileAdapter profiles, IActivityPubClient client)
    {
        _logger = logger;
        _profiles = profiles;
        _client = client;
    }

    public async Task DoWork(CloudEvent value)
    {
        _logger.LogDebug("Handle message {Type}", value.Type);
        if (value[IActivityMessageService.ProfileKey] is not string shortId ||
            value.Data is not string document ||
            value[IActivityMessageService.DestinationKey] is not string destination)
        {
            _logger.LogError("Couldn't process message {@Message}", value);
            return;
        }

        var profile = await _profiles.LookupProfile(Uuid7.FromId25String(shortId));

        var response = await _client.As(profile).SendDocument(new Uri(destination), document);
        _logger.LogDebug("Handled message {Type}, got response {Response}", value.Type, response);
    }
}