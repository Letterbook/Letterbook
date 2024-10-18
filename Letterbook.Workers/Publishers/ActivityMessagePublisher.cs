using ActivityPub.Types.AS;
using Letterbook.Core;
using Letterbook.Core.Adapters;
using Letterbook.Core.Models;
using Letterbook.Workers.Contracts;
using MassTransit;
using Microsoft.Extensions.Options;

namespace Letterbook.Workers.Publishers;

public class ActivityMessagePublisher : IActivityMessagePublisher
{
	private readonly CoreOptions _options;
	private readonly ILogger<ActivityMessagePublisher> _logger;
	private readonly IActivityPubDocument _document;
	private readonly IBus _bus;

	public ActivityMessagePublisher(ILogger<ActivityMessagePublisher> logger, IOptions<CoreOptions> options, IActivityPubDocument document, IBus bus)
	{
		_options = options.Value;
		_bus = bus;
		_logger = logger;
		_document = document;
	}

	/// <inheritdoc />
	public async Task Deliver(Uri inbox, ASType activity, Profile? onBehalfOf)
	{
		await _bus.Publish(FormatMessage(inbox, activity, onBehalfOf));
		_logger.LogInformation("Scheduled message type {Activity} for delivery to {Inbox}",
			activity.GetType(), inbox);
		_logger.LogDebug("Scheduled message type {Activity} from ({Thread})",
			activity.GetType(), Environment.CurrentManagedThreadId);
	}

	/// <inheritdoc />
	public async Task Publish(Uri inbox, Post post, Profile onBehalfOf, Mention? extraMention = default)
	{
		var doc = _document.FromPost(post);
		switch (extraMention?.Visibility)
		{
			case MentionVisibility.Bto:
				doc.BTo.Add(extraMention.Subject.FediId);
				break;
			case MentionVisibility.Bcc:
				doc.BCC.Add(extraMention.Subject.FediId);
				break;
			case MentionVisibility.To:
				doc.To.Add(extraMention.Subject.FediId);
				break;
			case MentionVisibility.Cc:
				doc.CC.Add(extraMention.Subject.FediId);
				break;
			case null:
			default:
				break;
		}
		var activity = _document.Create(onBehalfOf, doc);
		await Deliver(inbox, activity, onBehalfOf);
	}

	/// <inheritdoc />
	public async Task Follow(Uri inbox, Profile target, Profile actor)
	{
		var document = _document.Follow(actor, target, implicitId: true);
		await Deliver(inbox, document, actor);
	}

	/// <inheritdoc />
	public async Task Unfollow(Uri inbox, Profile target, Profile actor)
	{
		var document = _document.Undo(actor, _document.Follow(actor, target, implicitId: true));
		await Deliver(inbox, document, actor);
	}

	/// <inheritdoc />
	public async Task RemoveFollower(Uri inbox, Profile target, Profile actor) => await RejectFollower(inbox, target, actor);

	/// <inheritdoc />
	public async Task AcceptFollower(Uri inbox, Profile target, Profile actor)
	{
		var document = _document.Accept(actor, _document.Follow(target, actor));
		await Deliver(inbox, document, actor);
	}

	/// <inheritdoc />
	public async Task RejectFollower(Uri inbox, Profile target, Profile actor)
	{
		var document = _document.Reject(actor, _document.Follow(target, actor));
		await Deliver(inbox, document, actor);
	}

	public async Task PendingFollower(Uri inbox, Profile target, Profile actor)
	{
		var document = _document.TentativeAccept(actor, _document.Follow(target, actor));
		await Deliver(inbox, document, actor);
	}

	private ActivityMessage FormatMessage(Uri inbox, ASType activity, Profile? onBehalfOf)
	{
		var act = activity.Is<ASObject>(out var o)
			? o.Id
			: activity.Is<ASLink>(out var l)
				? l.HRef.ToString() : null;
		act ??= string.Join(',', activity.TypeMap.ASTypes);
		var data = _document.Serialize(activity);
		_logger.LogDebug("Scheduling message to {Inbox} with {Document}", inbox, data);
		return new ActivityMessage
		{
			Activity = act,
			Claims = [],
			Type = nameof(Deliver),
			Data = data,
			OnBehalfOf = onBehalfOf?.GetId(),
			Inbox = inbox
		};
	}
}