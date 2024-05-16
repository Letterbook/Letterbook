using CloudNative.CloudEvents;
using Letterbook.Core;
using Letterbook.Core.Adapters;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models;
using MassTransit;
using Microsoft.Extensions.Options;

namespace Letterbook.Workers.Publishers;

public class PostEventPublisher : IPostEventPublisher
{
	private readonly ILogger<PostEventPublisher> _logger;
	private readonly IBus _bus;
	private readonly CoreOptions _options;

	public PostEventPublisher(ILogger<PostEventPublisher> logger, IOptions<CoreOptions> options, IBus bus)
	{
		_logger = logger;
		_bus = bus;
		_options = options.Value;
	}

	/// <inheritdoc />
	public async Task Created(Post post)
	{
		var message = FormatMessageData(post, nameof(Created));
		await _bus.Publish(message);
		_logger.LogInformation("{Action} Post event {Id}", nameof(Created), message.Id);
	}

	/// <inheritdoc />
	public async Task Deleted(Post post)
	{
		var message = FormatMessageData(post, nameof(Deleted));
		await _bus.Publish(message);
		_logger.LogInformation("{Action} Post event {Id}", nameof(Deleted), message.Id);
	}

	/// <inheritdoc />
	public async Task Updated(Post post)
	{
		var message = FormatMessageData(post, nameof(Updated));
		await _bus.Publish(message);
		_logger.LogInformation("{Action} Post event {Id}", nameof(Updated), message.Id);
	}

	/// <inheritdoc />
	public async Task Published(Post post)
	{
		var message = FormatMessageData(post, nameof(Published));
		await _bus.Publish(message);
		_logger.LogInformation("{Action} Post event {Id}", nameof(Published), message.Id);
	}

	/// <inheritdoc />
	public async Task Received(Post post, Profile recipient)
	{
		var message = FormatMessageData(post, recipient.GetId25(), nameof(Received));
		await _bus.Publish(message);
		_logger.LogInformation("{Action} Post event {Id}", nameof(Received), message.Id);
	}

	/// <inheritdoc />
	public async Task Liked(Post post, Profile likedBy)
	{
		var message = FormatMessageData(post, likedBy.GetId25(), nameof(Liked));
		await _bus.Publish(message);
		_logger.LogInformation("{Action} Post event {Id}", nameof(Liked), message.Id);
	}

	/// <inheritdoc />
	public async Task Shared(Post post, Profile sharedBy)
	{
		var message = FormatMessageData(post, sharedBy.GetId25(), nameof(Shared));
		await _bus.Publish(message);
	}

	/*
	 * Private methods
	 */

	private CloudEvent FormatMessageData(Post value, string profileId, string action) =>
		FormatMessage(new IPostEventPublisher.Data
		{
			ProfileId = profileId,
			Post = value
		}, value.GetId25(), action);

	private CloudEvent FormatMessageData(Post value, string action) =>
		FormatMessage(new IPostEventPublisher.Data{Post = value}, value.GetId25(), action);

	private CloudEvent FormatMessage(IPostEventPublisher.Data data, string subject, string action)
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