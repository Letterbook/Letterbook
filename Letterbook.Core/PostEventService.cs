using CloudNative.CloudEvents;
using Letterbook.Core.Adapters;
using Letterbook.Core.Events;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Letterbook.Core;

public class PostEventService : IPostEvents
{
	private readonly ILogger<PostEventService> _logger;
	private readonly IBus _bus;
	private readonly CoreOptions _options;

	public PostEventService(ILogger<PostEventService> logger, IOptions<CoreOptions> options, IBus bus)
	{
		_logger = logger;
		_bus = bus;
		_options = options.Value;
	}

	/// <inheritdoc />
	public void Created(Post post)
	{
		var message = FormatMessageData(post, nameof(Created));
		_bus.Publish(message);
		_logger.LogInformation("{Action} Post event {Id}", nameof(Created), message.Id);
	}

	/// <inheritdoc />
	public void Deleted(Post post)
	{
		var message = FormatMessageData(post, nameof(Deleted));
		_bus.Publish(message);
		_logger.LogInformation("{Action} Post event {Id}", nameof(Deleted), message.Id);
	}

	/// <inheritdoc />
	public void Updated(Post post)
	{
		var message = FormatMessageData(post, nameof(Updated));
		_bus.Publish(message);
		_logger.LogInformation("{Action} Post event {Id}", nameof(Updated), message.Id);
	}

	/// <inheritdoc />
	public void Published(Post post)
	{
		var message = FormatMessageData(post, nameof(Published));
		_bus.Publish(message);
		_logger.LogInformation("{Action} Post event {Id}", nameof(Published), message.Id);
	}

	/// <inheritdoc />
	public void Received(Post post, Profile recipient)
	{
		var message = FormatMessageData(post, recipient.GetId25(), nameof(Received));
		_bus.Publish(message);
		_logger.LogInformation("{Action} Post event {Id}", nameof(Received), message.Id);
	}

	/// <inheritdoc />
	public void Liked(Post post, Profile likedBy)
	{
		var message = FormatMessageData(post, likedBy.GetId25(), nameof(Liked));
		_bus.Publish(message);
		_logger.LogInformation("{Action} Post event {Id}", nameof(Liked), message.Id);
	}

	/// <inheritdoc />
	public void Shared(Post post, Profile sharedBy)
	{
		var message = FormatMessageData(post, sharedBy.GetId25(), nameof(Shared));
		_bus.Publish(message);
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