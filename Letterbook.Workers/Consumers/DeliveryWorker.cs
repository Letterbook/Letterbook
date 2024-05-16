using CloudNative.CloudEvents;
using Letterbook.Core.Adapters;
using Letterbook.Core.Workers;
using Letterbook.Workers.Contracts;
using MassTransit;
using Medo;

namespace Letterbook.Workers.Consumers;

/// <summary>
/// Deliver ActivityPub messages to their destination
/// </summary>
public class DeliveryWorker : IObserverWorker, IConsumer<ActivityMessage>
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
		if (value[IActivityMessagePublisher.ProfileKey] is not string shortId ||
		    value.Data is not string document ||
		    value[IActivityMessagePublisher.DestinationKey] is not string destination)
		{
			_logger.LogError("Couldn't process message {@Message}", value);
			return;
		}

		var profile = await _profiles.LookupProfile(Uuid7.FromId25String(shortId));

		var response = await _client.As(profile).SendDocument(new Uri(destination), document);
		_logger.LogDebug("Handled message {Type}, got response {Response}", value.Type, response);
	}

	public async Task Consume(ConsumeContext<ActivityMessage> context)
	{
		var profile = await _profiles.LookupProfile(context.Message.OnBehalfOf);

		var response = await _client.As(profile).SendDocument(context.Message.Inbox, context.Message.Data);
		_logger.LogDebug("Handled message {Type}, got response {@Response}", context.Message.Type, response);
	}
}