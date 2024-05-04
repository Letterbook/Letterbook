using CloudNative.CloudEvents;
using Letterbook.Core.Adapters;
using Medo;
using Microsoft.Extensions.Logging;

namespace Letterbook.Core.Workers;

/// <summary>
/// Deliver ActivityPub messages to their destination
/// </summary>
public class DeliveryWorker : IObserverWorker
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

	public async Task DoWork(CloudEvent value, CancellationToken token)
	{
		_logger.LogDebug("Handle message {Type}", value.Type);
		if (value[IActivityMessage.ProfileKey] is not string shortId ||
		    value.Data is not string document ||
		    value[IActivityMessage.DestinationKey] is not string destination)
		{
			_logger.LogError("Couldn't process message {@Message}", value);
			return;
		}

		var profile = await _profiles.LookupProfile(Uuid7.FromId25String(shortId));

		var response = await _client.As(profile).SendDocument(new Uri(destination), document);
		_logger.LogDebug("Handled message {Type}, got response {Response}", value.Type, response);
	}
}