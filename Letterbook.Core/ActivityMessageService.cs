using ActivityPub.Types.AS;
using ActivityPub.Types.Conversion;
using CloudNative.CloudEvents;
using Letterbook.Core.Adapters;
using Letterbook.Core.Events;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Letterbook.Core;

public class ActivityMessageService : EventServiceBase<IActivityMessage>, IActivityMessage
{
	private readonly IJsonLdSerializer _serializer;
	private readonly CoreOptions _options;
	private readonly ILogger<ActivityMessageService> _logger;

	public ActivityMessageService(ILogger<ActivityMessageService> logger, IOptions<CoreOptions> options,
		IJsonLdSerializer serializer, IMessageBusAdapter messageBusAdapter) : base(messageBusAdapter)
	{
		_options = options.Value;
		_serializer = serializer;
		_logger = logger;
	}

	public void Deliver(Uri inbox, ASType activity, Profile? onBehalfOf)
	{
		_channel.OnNext(FormatMessage(inbox, activity, onBehalfOf));
		_logger.LogInformation("Scheduled message type {Activity} for delivery to {Inbox}",
			activity.GetType(), inbox);
		_logger.LogDebug("Scheduled message type {Activity} from ({Thread})",
			activity.GetType(), Environment.CurrentManagedThreadId);
	}

	private CloudEvent FormatMessage(Uri inbox, ASType activity, Profile? onBehalfOf)
	{
		var subject = activity.Is<ASObject>(out var o)
			? o.Id
			: activity.Is<ASLink>(out var l)
				? l.HRef.ToString()
				: string.Join(',', activity.TypeMap.ASTypes);
		return new CloudEvent
		{
			Id = Guid.NewGuid().ToString(),
			Source = _options.BaseUri(),
			Data = _serializer.Serialize(activity),
			Type = activity.GetType().ToString(),
			Subject = subject,
			Time = DateTimeOffset.UtcNow,
			[IActivityMessage.DestinationKey] = inbox.ToString(),
			[IActivityMessage.ProfileKey] = onBehalfOf?.GetId25(),
			["ltrauth"] = "",
		};
	}
}