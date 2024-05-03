using CloudNative.CloudEvents;
using Letterbook.Core.Adapters;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Letterbook.Core;

public class PostEventService : IPostEventService
{
	private readonly ILogger<PostEventService> _logger;
	private readonly CoreOptions _options;
	private IObserver<CloudEvent> _channel;

	public PostEventService(ILogger<PostEventService> logger, IOptions<CoreOptions> options, IMessageBusAdapter messageBusAdapter)
	{
		_logger = logger;
		_options = options.Value;
		_channel = messageBusAdapter.OpenChannel<Post>(nameof(PostEventService));
	}

	public void Created(Post post)
	{
		var message = FormatMessage(post, nameof(Created));
		_channel.OnNext(message);
	}

	public void Deleted(Post post)
	{
		var message = FormatMessage(post, nameof(Deleted));
		_channel.OnNext(message);
	}

	public void Updated(Post post)
	{
		var message = FormatMessage(post, nameof(Updated));
		_channel.OnNext(message);
	}

	public void Published(Post post)
	{
		var message = FormatMessage(post, nameof(Published));
		_channel.OnNext(message);
	}

	public void Received(Post post, Profile actor)
	{
		var message = FormatMessage(post, nameof(Received));
		_channel.OnNext(message);
	}

	public void Liked(Post post, Profile actor)
	{
		var message = FormatMessage(post, nameof(Liked));
		_channel.OnNext(message);
	}

	public void Shared(Post post, Profile actor)
	{
		var message = FormatMessage(post, nameof(Shared));
		_channel.OnNext(message);
	}

	private IObserver<CloudEvent> GetChannel() => _channel;

	private CloudEvent FormatMessage(Post value, string action)
	{
		return new CloudEvent
		{
			Id = Guid.NewGuid().ToString(),
			Source = _options.BaseUri(),
			Data = value,
			Type = $"{nameof(Post)}.{action}",
			Subject = value.Id.ToString(),
			Time = DateTimeOffset.UtcNow
		};
	}
}