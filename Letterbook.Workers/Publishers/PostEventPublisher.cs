using System.Collections.Immutable;
using AutoMapper;
using Letterbook.Core;
using Letterbook.Core.Adapters;
using Letterbook.Core.Models;
using Letterbook.Core.Models.Dto;
using Letterbook.Core.Models.Mappers;
using Letterbook.Workers.Contracts;
using MassTransit;
using Medo;
using Microsoft.Extensions.Options;
using Claim = Letterbook.Workers.Contracts.Claim;
using Profile = Letterbook.Core.Models.Profile;

namespace Letterbook.Workers.Publishers;

public class PostEventPublisher : IPostEventPublisher
{
	private readonly ILogger<PostEventPublisher> _logger;
	private readonly IBus _bus;
	private readonly CoreOptions _options;
	private readonly Mapper _mapper;

	public PostEventPublisher(ILogger<PostEventPublisher> logger, IOptions<CoreOptions> options, IBus bus, MappingConfigProvider maps)
	{
		_logger = logger;
		_bus = bus;
		_options = options.Value;
		_mapper = new Mapper(maps.Posts);
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

	private PostEvent Message(Post value, Uuid7? sender, string action, ImmutableHashSet<Claim> claims) =>
		Message(value, null, sender, action, claims);

	private PostEvent Message(Post nextValue, Post? prevValue, Uuid7? sender, string action, ImmutableHashSet<Claim> claims) =>
		new PostEvent
		{
			Sender = sender,
			Claims = claims.ToArray(),
			NextData = _mapper.Map<PostDto>(nextValue),
			PrevData = prevValue is null ? null : _mapper.Map<PostDto>(prevValue),
			Subject = nextValue.GetId25(),
			Type = action
		};
}