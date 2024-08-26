using ActivityPub.Types.AS;
using AutoMapper;
using Letterbook.Core;
using Letterbook.Core.Adapters;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models;
using Letterbook.Core.Models.Mappers;
using Letterbook.Workers.Contracts;
using Letterbook.Workers.Publishers;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Profile = Letterbook.Core.Models.Profile;

namespace Letterbook.Workers.Consumers;

public class OutboundPostConsumer : IConsumer<PostEvent>
{
	private readonly ILogger<OutboundPostConsumer> _logger;
	private readonly IActivityMessagePublisher _messagePublisher;
	private readonly IPostAdapter _posts;
	private readonly IProfileService _profileService;
	private readonly IActivityPubDocument _document;
	private readonly Mapper _mapper;
	private readonly CoreOptions _config;

	public OutboundPostConsumer(ILogger<OutboundPostConsumer> logger, IOptions<CoreOptions> coreOptions, MappingConfigProvider maps,
		IActivityMessagePublisher messagePublisher, IPostAdapter posts, IProfileService profileService, IActivityPubDocument document)
	{
		_logger = logger;
		_messagePublisher = messagePublisher;
		_posts = posts;
		_profileService = profileService;
		_document = document;
		_mapper = new Mapper(maps.Posts);
		_config = coreOptions.Value;

	}

	public async Task Consume(ConsumeContext<PostEvent> context)
	{
		_logger.LogInformation("Handling PostEvent {EventType} for {PostId}", context.Message.Type, context.Message.Subject);
		var post = _mapper.Map<Post>(context.Message.NextData);
		if (!post.FediId.HasLocalAuthority(_config)) return;

		var sender = context.Message.Sender is { } id ? await _profileService.As(context.Message.Claims).LookupProfile(id) : default;
		switch (context.Message.Type)
		{
			case nameof(PostEventPublisher.Published):
				var mentions = await GetMentionedProfiles(post).ToListAsync();
				// TODO: build real doc, with tags
				var doc = _mapper.Map<ASType>(post);

				// Deliver directly to mentioned profiles
				foreach (var mention in mentions)
				{
					await _messagePublisher.Deliver(mention.Inbox, doc, sender);
				}

				// Deliver to audience, but don't double post to the mentioned profiles
				await foreach (var inbox in GetAudienceInboxes(post)
					               .Where(inbox => !mentions.Select(mention => mention.Inbox).Contains(inbox)))
				{
					await _messagePublisher.Deliver(inbox, doc, sender);
				}
				throw new NotImplementedException();
			case nameof(PostEventPublisher.Updated):
				throw new NotImplementedException();
			case nameof(PostEventPublisher.Shared):
				throw new NotImplementedException();
			case nameof(PostEventPublisher.Deleted):
				throw new NotImplementedException();
			default:
				return;
		}
	}

	private IAsyncEnumerable<Profile> GetMentionedProfiles(Post post)
	{
		return _posts.QueryFrom(post, p => p.AddressedTo)
			.Include(mention => mention.Subject)
			.Select(mention => mention.Subject)
			.Where(subject => !subject.Authority.StartsWith(_config.BaseUri().GetAuthority()))
			.AsAsyncEnumerable();
	}

	private IAsyncEnumerable<Uri> GetAudienceInboxes(Post post)
	{
		return _posts.QueryFrom(post, p => p.Audience)
				.Include(audience => audience.Members)
				.SelectMany(audience => audience.Members)
				.Where(member => !member.Authority.StartsWith(_config.BaseUri().GetAuthority()))
				.Select(member => member.SharedInbox ?? member.Inbox)
				.Distinct()
				.AsAsyncEnumerable();
	}
}