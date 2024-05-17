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
public class DeliveryWorker : IConsumer<ActivityMessage>
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

	public async Task Consume(ConsumeContext<ActivityMessage> context)
	{
		var profile = await _profiles.LookupProfile(context.Message.OnBehalfOf);

		var response = await _client.As(profile).SendDocument(context.Message.Inbox, context.Message.Data);
		_logger.LogDebug("Handled message {Type}, got response {@Response}", context.Message.Type, response);
	}
}