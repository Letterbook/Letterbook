using CloudNative.CloudEvents;
using Letterbook.Core.Adapters;
using Letterbook.Core.Events;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Letterbook.Core;

public class PostEventService : EventServiceBase<IPostEvents>, IPostEvents
{
	private readonly ILogger<PostEventService> _logger;
	private readonly CoreOptions _options;

	public PostEventService(ILogger<PostEventService> logger, IOptions<CoreOptions> options, IMessageBusAdapter messageBusAdapter)
	: base(messageBusAdapter)
	{
		_logger = logger;
		_options = options.Value;
	}

	/// <inheritdoc />
	public void Created(Post post)
	{
		var message = FormatMessageData(post, nameof(Created));
		_channel.OnNext(message);
		_logger.LogInformation("{Action} Post event {Id}", nameof(Created), message.Id);
	}

	/// <inheritdoc />
	public void Deleted(Post post)
	{
		var message = FormatMessageData(post, nameof(Deleted));
		_channel.OnNext(message);
		_logger.LogInformation("{Action} Post event {Id}", nameof(Deleted), message.Id);
	}

	/// <inheritdoc />
	public void Updated(Post post)
	{
		var message = FormatMessageData(post, nameof(Updated));
		_channel.OnNext(message);
		_logger.LogInformation("{Action} Post event {Id}", nameof(Updated), message.Id);
	}

	/// <inheritdoc />
	public void Published(Post post)
	{
		var message = FormatMessageData(post, nameof(Published));
		_channel.OnNext(message);
		_logger.LogInformation("{Action} Post event {Id}", nameof(Published), message.Id);
	}

	/// <inheritdoc />
	public void Received(Post post, Profile recipient)
	{
		var message = FormatMessageData(post, recipient.GetId25(), nameof(Received));
		_channel.OnNext(message);
		_logger.LogInformation("{Action} Post event {Id}", nameof(Received), message.Id);
	}

	/// <inheritdoc />
	public void Liked(Post post, Profile likedBy)
	{
		var message = FormatMessageData(post, likedBy.GetId25(), nameof(Liked));
		_channel.OnNext(message);
		_logger.LogInformation("{Action} Post event {Id}", nameof(Liked), message.Id);
	}

	/// <inheritdoc />
	public void Shared(Post post, Profile sharedBy)
	{
		var message = FormatMessageData(post, sharedBy.GetId25(), nameof(Shared));
		_channel.OnNext(message);
	}

	/*
	 * Private methods
	 */

	private CloudEvent FormatMessageData(Post value, string profileId, string action) =>
		FormatMessage(new IPostEvents.Data
		{
			ProfileId = profileId,
			Post = value
		}, value.GetId25(), action);

	private CloudEvent FormatMessageData(Post value, string action) =>
		FormatMessage(new IPostEvents.Data{Post = value}, value.GetId25(), action);

	private CloudEvent FormatMessage(IPostEvents.Data data, string subject, string action)
	{
		return new CloudEvent
		{
			Id = Guid.NewGuid().ToString(),
			Source = _options.BaseUri(),
			Data = data,
			Type = $"{nameof(Post)}.{action}",
			Subject = subject,
			Time = DateTimeOffset.UtcNow
		};
	}
}