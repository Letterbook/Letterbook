using ActivityPub.Types.AS;
using ActivityPub.Types.Conversion;
using CloudNative.CloudEvents;
using Letterbook.Core;
using Letterbook.Core.Adapters;
using Letterbook.Core.Events;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models;
using Letterbook.Workers.Contracts;
using MassTransit;
using Microsoft.Extensions.Options;

namespace Letterbook.Workers.Publishers;

public class ActivityMessagePublisher : IActivityMessagePublisher
{
	private readonly IJsonLdSerializer _serializer;
	private readonly CoreOptions _options;
	private readonly ILogger<ActivityMessagePublisher> _logger;
	private readonly IBus _bus;

	public ActivityMessagePublisher(ILogger<ActivityMessagePublisher> logger, IOptions<CoreOptions> options,
		IJsonLdSerializer serializer, IBus bus)
	{
		_options = options.Value;
		_serializer = serializer;
		_bus = bus;
		_logger = logger;
	}

	public async Task Deliver(Uri inbox, ASType activity, Profile? onBehalfOf)
	{
		await _bus.Publish(FormatMessage(inbox, activity, onBehalfOf));
		_logger.LogInformation("Scheduled message type {Activity} for delivery to {Inbox}",
			activity.GetType(), inbox);
		_logger.LogDebug("Scheduled message type {Activity} from ({Thread})",
			activity.GetType(), Environment.CurrentManagedThreadId);
	}

	private ActivityMessage FormatMessage(Uri inbox, ASType activity, Profile? onBehalfOf)
	{
		var subject = activity.Is<ASObject>(out var o)
			? o.Id
			: activity.Is<ASLink>(out var l)
				? l.HRef.ToString()
				: string.Join(',', activity.TypeMap.ASTypes);
		return new ActivityMessage
		{
			Source = _options.BaseUri().ToString(),
			Subject = subject,
			Claims = [],
			Type = nameof(Deliver),
			NextData = _serializer.Serialize(activity),
			OnBehalfOf = onBehalfOf?.GetId(),
			Inbox = inbox
		};

		// return new CloudEvent
		// {
		// 	Id = Guid.NewGuid().ToString(),
		// 	Source = _options.BaseUri(),
		// 	Data = _serializer.Serialize(activity),
		// 	Type = activity.GetType().ToString(),
		// 	Subject = subject,
		// 	Time = DateTimeOffset.UtcNow,
		// 	[IActivityMessagePublisher.DestinationKey] = inbox.ToString(),
		// 	[IActivityMessagePublisher.ProfileKey] = onBehalfOf?.GetId25(),
		// 	["ltrauth"] = "",
		// };
	}
}