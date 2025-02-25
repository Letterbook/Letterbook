using System.Diagnostics;
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
using Claim = System.Security.Claims.Claim;
using Profile = Letterbook.Core.Models.Profile;

namespace Letterbook.Workers.Consumers;

public class OutboundPostConsumer : IConsumer<PostEvent>
{
	private readonly ILogger<OutboundPostConsumer> _logger;
	private readonly Instrumentation _instrumentation;
	private readonly IActivityScheduler _scheduler;
	private readonly IDataAdapter _posts;
	private readonly IProfileService _profileService;
	private readonly IActivityPubDocument _document;
	private readonly Mapper _mapper;
	private readonly CoreOptions _config;

	public OutboundPostConsumer(ILogger<OutboundPostConsumer> logger, IOptions<CoreOptions> coreOptions, MappingConfigProvider maps,
		Instrumentation instrumentation, IActivityScheduler scheduler, IDataAdapter posts, IProfileService profileService,
		IActivityPubDocument document)
	{
		_logger = logger;
		_instrumentation = instrumentation;
		_scheduler = scheduler;
		_posts = posts;
		_profileService = profileService;
		_document = document;
		_mapper = new Mapper(maps.Posts);
		_config = coreOptions.Value;

	}

	public async Task Consume(ConsumeContext<PostEvent> context)
	{
		using var span = Activity.Current;
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
				await DeliverAudienceAndMentions(post, sender, _scheduler.Publish);
				break;
			case nameof(PostEventPublisher.Updated):
				await DeliverAudienceAndMentions(post, sender, _scheduler.Update);
				break;
			case nameof(PostEventPublisher.Shared):
				await DeliverFollowers(post, sender, _scheduler.Share);
				break;
			case nameof(PostEventPublisher.Deleted):
				var mentions = await GetMentionedProfiles(post).ToListAsync();
				foreach (var mention in mentions)
				{
					await _scheduler.Delete(mention.Subject.Inbox, post, sender);
				}
				await DeliverAudience(post, sender, _scheduler.Delete);
				break;
			case nameof(PostEventPublisher.Liked):
				await foreach (var inbox in GetAuthorsInboxes(post))
				{
					await _scheduler.Like(inbox, post, sender);
				}
				break;
			default:
				span?.AddEvent(new("unhandledEvent", DateTime.UtcNow, new ActivityTagsCollection
				{
					{"Type", context.Message.Type}
				}));
				return;
		}
	}

	/// <summary>
	/// Schedule delivery to a Post's Audience members and mentions.
	/// </summary>
	/// <remarks>The message is delivered individually to the direct mentions, with customization to preserve the privacy
	/// of blind mentions.</remarks>
	/// <param name="post"></param>
	/// <param name="sender"></param>
	/// <param name="fn">A function that will schedule the relevant delivery type</param>
	private async Task DeliverAudienceAndMentions(Post post, Profile sender, Func<Uri, Post, Profile, Mention?, Task> fn)
	{
		using var span = _instrumentation.Span<OutboundPostConsumer>();
		var mentions = await GetMentionedProfiles(post).ToListAsync();
		span?.AddTag("mentions.inboxes", string.Join(", ", mentions.Select(m => m.Subject.Inbox)));

		// Deliver directly to mentioned profiles
		foreach (var mention in mentions)
		{
			switch (mention.Visibility)
			{
				case MentionVisibility.Bto:
				case MentionVisibility.Bcc:
					await fn(mention.Subject.Inbox, post, sender, mention);
					break;
				case MentionVisibility.To:
				case MentionVisibility.Cc:
					await fn(mention.Subject.Inbox, post, sender, null);
					break;
				default:
					break;
			}
		}

		// Deliver to audience, but don't double post to the mentioned profiles
		await foreach (var inbox in GetAudienceInboxes(post)
			               .Where(inbox => !mentions.Select(mention => mention.Subject.Inbox).Contains(inbox)))
		{
			await fn(inbox, post, sender, null);
		}
	}

	private async Task DeliverFollowers(Post post, Profile sender, Func<Uri, Post, Profile, Task> fn)
	{
		using var span = Activity.Current;
		span?.AddTag("audience.ids", string.Join(", ", post.Audience.Select(a => a.FediId)));

		await foreach (var inbox in GetFollowerInboxes(sender))
		{
			await fn(inbox, post, sender);
		}
	}

	/// <summary>
	/// Schedule delivery to a Post's Audience members
	/// </summary>
	/// <param name="post"></param>
	/// <param name="sender"></param>
	/// <param name="fn">A function that will schedule the relevant delivery type</param>
	private async Task DeliverAudience(Post post, Profile sender, Func<Uri, Post, Profile, Task> fn)
	{
		using var span = Activity.Current;
		span?.AddTag("audience.ids", string.Join(", ", post.Audience.Select(a => a.FediId)));

		var inboxes = await GetAudienceInboxes(post).ToListAsync();
		span?.AddEvent(new ActivityEvent("inboxes", DateTimeOffset.UtcNow, new ActivityTagsCollection()
		{
			{"count", inboxes.Count}
		}));
		foreach (var inbox in inboxes)
		{
			await fn(inbox, post, sender);
		}
	}

	private IAsyncEnumerable<Mention> GetMentionedProfiles(Post post)
	{
		return _posts.QueryFrom(post, p => p.AddressedTo)
			.Include(mention => mention.Subject)
			.Where(mention => !mention.Subject.Authority.StartsWith(_config.BaseUri().GetAuthority()))
			.TagWithCallSite()
			.TagWith(nameof(GetMentionedProfiles))
			.AsNoTracking()
			.AsSplitQuery()
			.AsAsyncEnumerable();
	}

	private IAsyncEnumerable<Uri> GetAuthorsInboxes(Post post)
	{
		return _posts.QueryFrom(post, p => p.Creators)
			.Where(profile => !profile.Authority.StartsWith(_config.BaseUri().GetAuthority()))
			.Select(profile => profile.Inbox)
			.TagWithCallSite()
			.TagWith(nameof(GetAuthorsInboxes))
			.AsNoTracking()
			.AsSplitQuery()
			.AsAsyncEnumerable();
	}

	private IAsyncEnumerable<Uri> GetAudienceInboxes(Post post)
	{
		_logger.LogDebug("Getting inboxes for {Audiences}", post.Audience.Select(a => a.FediId));
		return _posts.Audiences(post.Audience.Select(a => a.FediId).ToArray())
			.Include(audience => audience.Members)
			.SelectMany(profile => profile.Members)
			.Select(member => member.SharedInbox ?? member.Inbox)
			.Distinct()
			.TagWith(nameof(GetAudienceInboxes))
			.AsNoTracking()
			.AsSplitQuery()
			.AsAsyncEnumerable();
	}

	private IAsyncEnumerable<Uri> GetFollowerInboxes(Profile profile)
	{
		_logger.LogDebug("Getting follower inboxes for {Profile}", profile.GetId25());
		return _posts.QueryFrom(profile, p => p.Headlining)
			.Where(audience => audience.FediId == Audience.Followers(profile).FediId)
			.Include(audience => audience.Members)
			.SelectMany(audience => audience.Members)
			.Where(member => !member.Authority.StartsWith(_config.BaseUri().GetAuthority()))
			.Select(member => member.SharedInbox ?? member.Inbox)
			.Distinct()
			.TagWithCallSite()
			.TagWith(nameof(GetFollowerInboxes))
			.AsNoTracking()
			.AsSplitQuery()
			.AsAsyncEnumerable();
	}
}