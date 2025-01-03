﻿using AutoMapper;
using Letterbook.Core;
using Letterbook.Core.Adapters;
using Letterbook.Core.Models;
using Letterbook.Core.Models.Dto;
using Letterbook.Core.Models.Mappers;
using Letterbook.Workers.Contracts;
using MassTransit;
using Medo;
using Microsoft.Extensions.Options;
using Claim = System.Security.Claims.Claim;

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
	public async Task Created(Post post, Uuid7 createdBy, IEnumerable<Claim> claims)
	{
		var message = Message(post, nameof(Created), claims, createdBy);
		await _bus.Publish(message, c => Headers(c, nameof(Created)));
	}

	/// <inheritdoc />
	public async Task Deleted(Post post, Uuid7 deletedBy, IEnumerable<Claim> claims)
	{
		var message = Message(post, nameof(Deleted), claims, deletedBy);
		await _bus.Publish(message, c => Headers(c, nameof(Deleted)));
	}

	/// <inheritdoc />
	public async Task Updated(Post post, Uuid7 updatedBy, IEnumerable<Claim> claims)
	{
		var message = Message(post, nameof(Updated), claims, updatedBy);
		await _bus.Publish(message, c => Headers(c, nameof(Updated)));
	}

	/// <inheritdoc />
	public async Task Published(Post post, Uuid7 publishedBy, IEnumerable<Claim> claims)
	{
		var message = Message(post, nameof(Published), claims, publishedBy);
		await _bus.Publish(message, c => Headers(c, nameof(Published)));
	}

	/// <inheritdoc />
	public async Task Liked(Post post, Uuid7 likedBy, IEnumerable<Claim> claims)
	{
		var message = Message(post, likedBy, nameof(Liked), claims);
		await _bus.Publish(message, c => Headers(c, nameof(Liked)));
	}

	/// <inheritdoc />
	public async Task Shared(Post post, Uuid7 sharedBy, IEnumerable<Claim> claims)
	{
		var message = Message(post, sharedBy, nameof(Shared), claims);
		await _bus.Publish(message, c => Headers(c, nameof(Shared)));
	}

	/*
	 * Private methods
	 */

	private static void Headers(PublishContext<PostEvent> context, string value)
	{
		context.Headers.Set(Workers.Headers.Event, value);
	}

	private PostEvent Message(Post value, string action, IEnumerable<Claim> claims, Uuid7 sender) => Message(value, sender, action, claims);

	private PostEvent Message(Post value, Uuid7 sender, string action, IEnumerable<Claim> claims) =>
		Message(value, null, sender, action, claims);

	private PostEvent Message(Post nextValue, Post? prevValue, Uuid7 sender, string action, IEnumerable<Claim> claims) =>
		new PostEvent
		{
			Sender = sender,
			Claims = claims.Select(c => (Contracts.Claim)c).ToArray(),
			NextData = _mapper.Map<PostDto>(nextValue),
			PrevData = prevValue is null ? null : _mapper.Map<PostDto>(prevValue),
			Subject = nextValue.GetId25(),
			Type = action
		};
}