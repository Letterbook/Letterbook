using AutoMapper;
using Letterbook.Adapter.ActivityPub;
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
using Claim = System.Security.Claims.Claim;
using Profile = Letterbook.Core.Models.Profile;

namespace Letterbook.Workers.Consumers;

public class OutboundPostConsumer : IConsumer<PostEvent>
{
	private readonly ILogger<OutboundPostConsumer> _logger;
	private readonly IActivityMessagePublisher _messagePublisher;
	private readonly IDataAdapter _posts;
	private readonly IProfileService _profileService;
	private readonly IActivityPubDocument _document;
	private readonly Mapper _mapper;
	private readonly CoreOptions _config;

	public OutboundPostConsumer(ILogger<OutboundPostConsumer> logger, IOptions<CoreOptions> coreOptions, MappingConfigProvider maps,
		IActivityMessagePublisher messagePublisher, IDataAdapter posts, IProfileService profileService, IActivityPubDocument document)
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
		var post = _mapper.Map<Post>(context.Message.NextData);
		if (!post.FediId.HasLocalAuthority(_config)) return;
		_logger.LogInformation("Handling PostEvent {EventType} for {PostId}", context.Message.Type, context.Message.Subject);

		var svc = _profileService.As(context.Message.Claims.Select(c => (Claim)c));
		if (await svc.LookupProfile(context.Message.Sender) is not { } sender)
		{
			_logger.LogError("Sender not found for {@Event}", context.Message);
			return;
		}

		switch (context.Message.Type)
		{
			case nameof(PostEventPublisher.Published):
				await Published(post, sender);
				break;
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

	private async Task Published(Post post, Profile sender)
	{
		var mentions = await GetMentionedProfiles(post).ToListAsync();
		var doc = _document.FromPost(post);

		// Deliver directly to mentioned profiles
		foreach (var mention in mentions)
		{
			var privateDoc = doc;
			switch (mention.Visibility)
			{
				case MentionVisibility.Bto:
				case MentionVisibility.Bcc:
					privateDoc = _document.FromPost(post);
					privateDoc.Mention(mention);
					break;
				case MentionVisibility.To:
				case MentionVisibility.Cc:
				default:
					break;
			}
			await _messagePublisher.Deliver(mention.Subject.Inbox, _document.Create(sender, privateDoc), sender);
		}

		// Deliver to audience, but don't double post to the mentioned profiles
		await foreach (var inbox in GetAudienceInboxes(post)
			               .Where(inbox => !mentions.Select(mention => mention.Subject.Inbox).Contains(inbox)))
		{
			await _messagePublisher.Deliver(inbox, doc, sender);
		}
	}

	private IAsyncEnumerable<Mention> GetMentionedProfiles(Post post)
	{
		return _posts.QueryFrom(post, p => p.AddressedTo)
			.Include(mention => mention.Subject)
			.Where(mention => !mention.Subject.Authority.StartsWith(_config.BaseUri().GetAuthority()))
			.AsAsyncEnumerable();
	}

	private IAsyncEnumerable<Uri> GetAudienceInboxes(Post post)
	{
		_logger.LogDebug("Getting inboxes for {Audiences}", post.Audience.Select(a => a.FediId));
		return _posts.QueryFrom(post, p => p.Audience)
				.Include(audience => audience.Members)
				.SelectMany(audience => audience.Members)
				.Where(member => !member.Authority.StartsWith(_config.BaseUri().GetAuthority()))
				.Select(member => member.SharedInbox ?? member.Inbox)
				.Distinct()
				.AsAsyncEnumerable();
	}
}