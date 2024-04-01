using CloudNative.CloudEvents;
using Letterbook.Core.Adapters;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models;
using Microsoft.Extensions.Options;

namespace Letterbook.Core;

/// <summary>
/// ActivityEventService provides a loosely coupled publish/subscribe interface between other core services. Typically,
/// core services will be narrowly scoped, and will publish a message through the event service. Other services may
/// consume that message and perform their own processing in response. For instance, to send notifications.
/// </summary>
public class ActivityEventService : IActivityEventService
{
	private readonly CoreOptions _options;
	private readonly IMessageBusAdapter _messageBusAdapter;
	private readonly IObserver<CloudEvent> _channel;

	public ActivityEventService(IOptions<CoreOptions> options, IMessageBusAdapter messageBusAdapter)
	{
		_options = options.Value;
		_messageBusAdapter = messageBusAdapter;
		_channel = _messageBusAdapter.OpenChannel<Post>(nameof(ActivityEventService));
	}

	public void Created(Post value)
	{
		var message = FormatMessage(value, nameof(Created));
		var channel = GetChannel();
		channel.OnNext(message);
	}

	public void Updated(Post value)
	{
		var message = FormatMessage(value, nameof(Updated));
		var channel = GetChannel();
		channel.OnNext(message);
	}

	public void Deleted(Post value)
	{
		var message = FormatMessage(value, nameof(Deleted));
		var channel = GetChannel();
		channel.OnNext(message);
	}

	public void Flagged(Post value)
	{
		var message = FormatMessage(value, nameof(Flagged));
		var channel = GetChannel();
		channel.OnNext(message);
	}

	public void Liked(Post value)
	{
		var message = FormatMessage(value, nameof(Liked));
		var channel = GetChannel();
		channel.OnNext(message);
	}

	public void Boosted(Post value)
	{
		var message = FormatMessage(value, nameof(Boosted));
		var channel = GetChannel();
		channel.OnNext(message);
	}

	public void Approved(Post value)
	{
		var message = FormatMessage(value, nameof(Approved));
		var channel = GetChannel();
		channel.OnNext(message);
	}

	public void Rejected(Post value)
	{
		var message = FormatMessage(value, nameof(Rejected));
		var channel = GetChannel();
		channel.OnNext(message);
	}

	public void Requested(Post value)
	{
		var message = FormatMessage(value, nameof(Requested));
		var channel = GetChannel();
		channel.OnNext(message);
	}

	public void Offered(Post value)
	{
		var message = FormatMessage(value, nameof(Offered));
		var channel = GetChannel();
		channel.OnNext(message);
	}

	public void Mentioned(Post value)
	{
		var message = FormatMessage(value, nameof(Mentioned));
		var channel = GetChannel();
		channel.OnNext(message);
	}

	private IObserver<CloudEvent> GetChannel() => _channel;

	private CloudEvent FormatMessage(Post value, string action)
	{
		return new CloudEvent
		{
			Id = Guid.NewGuid().ToString(),
			Source = _options.BaseUri(),
			Data = value,
			Type = $"{nameof(ActivityEventService)}.{value.GetType()}.{action}",
			Subject = value.Id.ToString(),
			Time = DateTimeOffset.UtcNow
		};
	}
}