using ActivityPub.Types.AS;
using Letterbook.Adapter.ActivityPub;
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
	public async Task Follow(Uri inbox, Profile target, Profile actor)
	{
		var document = _document.Follow(actor, target);
		await Deliver(inbox, document, actor);
	}

	/// <inheritdoc />
	public async Task Unfollow(Uri inbox, Profile target, Profile actor)
	{
		var document = _document.Undo(actor, _document.Follow(actor, target));
		await Deliver(inbox, document, actor);
	}

	/// <inheritdoc />
	public async Task RemoveFollower(Uri inbox, Profile target, Profile actor)
	{
		var document = _document.Undo(actor, _document.Accept(actor, _document.Follow(target, actor)));
		await Deliver(inbox, document, actor);
	}

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
		var subject = activity.Is<ASObject>(out var o)
			? o.Id
			: activity.Is<ASLink>(out var l)
				? l.HRef.ToString() : null;
		subject ??= string.Join(',', activity.TypeMap.ASTypes);
		return new ActivityMessage
		{
			Subject = subject,
			Claims = [],
			Type = nameof(Deliver),
			Data = _document.Serialize(activity),
			OnBehalfOf = onBehalfOf?.GetId(),
			Inbox = inbox
		};
	}
}