using System.Collections.Immutable;
using System.Security.Claims;
using Letterbook.Core;
using Letterbook.Core.Adapters;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models;
using Letterbook.Workers.Contracts;
using MassTransit;
using Medo;
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
		var message = Message(post, nameof(Created), []);
		await _bus.Publish(message);
	}

	/// <inheritdoc />
	public async Task Deleted(Post post)
	{
		var message = Message(post, nameof(Deleted), []);
		await _bus.Publish(message);
	}

	/// <inheritdoc />
	public async Task Updated(Post post)
	{
		var message = Message(post, nameof(Updated), []);
		await _bus.Publish(message);
	}

	/// <inheritdoc />
	public async Task Published(Post post)
	{
		var message = Message(post, nameof(Published), []);
		await _bus.Publish(message);
	}

	/// <inheritdoc />
	public async Task Liked(Post post, Profile likedBy)
	{
		var message = Message(post, likedBy.GetId(), nameof(Liked), []);
		await _bus.Publish(message);
	}

	/// <inheritdoc />
	public async Task Shared(Post post, Profile sharedBy)
	{
		var message = Message(post, sharedBy.GetId(), nameof(Shared), []);
		await _bus.Publish(message);
	}

	/*
	 * Private methods
	 */

	private PostEvent Message(Post value, string action, ImmutableHashSet<Claim> claims) => Message(value, null, action, claims);

	private PostEvent Message(Post value, Uuid7? sender, string action, ImmutableHashSet<Claim> claims) => Message(value, null, sender, action, claims);

	private PostEvent Message(Post nextValue, Post? prevValue, Uuid7? sender, string action, ImmutableHashSet<Claim> claims) =>
		new PostEvent
		{
			Source = _options.BaseUri().ToString(),
			Sender = sender,
			Claims = claims,
			Type = action,
			NextData = nextValue,
			PrevData = prevValue,
			Subject = nextValue.GetId25()
		};
}