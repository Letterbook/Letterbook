using System.Collections.Immutable;
using System.Security.Claims;
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
	public async Task Created(Post post, IEnumerable<Claim> claims)
	{
		var message = Message(post, nameof(Created), claims);
		await _bus.Publish(message);
	}

	/// <inheritdoc />
	public async Task Deleted(Post post, IEnumerable<Claim> claims)
	{
		var message = Message(post, nameof(Deleted), claims);
		await _bus.Publish(message);
	}

	/// <inheritdoc />
	public async Task Updated(Post post, IEnumerable<Claim> claims)
	{
		var message = Message(post, nameof(Updated), claims);
		await _bus.Publish(message);
	}

	/// <inheritdoc />
	public async Task Published(Post post, IEnumerable<Claim> claims)
	{
		var message = Message(post, nameof(Published), claims);
		await _bus.Publish(message);
	}

	/// <inheritdoc />
	public async Task Liked(Post post, Profile likedBy, IEnumerable<Claim> claims)
	{
		var message = Message(post, likedBy.GetId(), nameof(Liked), claims);
		await _bus.Publish(message);
	}

	/// <inheritdoc />
	public async Task Shared(Post post, Profile sharedBy, IEnumerable<Claim> claims)
	{
		var message = Message(post, sharedBy.GetId(), nameof(Shared), claims);
		await _bus.Publish(message);
	}

	/*
	 * Private methods
	 */

	private PostEvent Message(Post value, string action, IEnumerable<Claim> claims) => Message(value, null, action, claims);

	private PostEvent Message(Post value, Uuid7? sender, string action, IEnumerable<Claim> claims) =>
		Message(value, null, sender, action, claims);

	private PostEvent Message(Post nextValue, Post? prevValue, Uuid7? sender, string action, IEnumerable<Claim> claims) =>
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