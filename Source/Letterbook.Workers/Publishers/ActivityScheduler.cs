using System.Text.Json;
using System.Text.Json.Nodes;
using ActivityPub.Types.AS;
using ActivityPub.Types.AS.Extended.Activity;
using ActivityPub.Types.Conversion;
using Letterbook.Core;
using Letterbook.Core.Adapters;
using Letterbook.Core.Models;
using Letterbook.Workers.Contracts;
using MassTransit;
using Microsoft.Extensions.Options;
using Claim = System.Security.Claims.Claim;

namespace Letterbook.Workers.Publishers;

public class ActivityScheduler : IActivityScheduler
{
	private readonly CoreOptions _options;
	private readonly ILogger<ActivityScheduler> _logger;
	private readonly IActivityPubDocument _document;
	private readonly IBus _bus;
	private readonly IAuthorizationService _authz;

	public ActivityScheduler(ILogger<ActivityScheduler> logger, IOptions<CoreOptions> options, IActivityPubDocument document, IBus bus,
		IAuthorizationService authz)
	{
		_options = options.Value;
		_bus = bus;
		_authz = authz;
		_logger = logger;
		_document = document;
	}

	/// <inheritdoc />
	public async Task Deliver(Uri inbox, ASType activity, IEnumerable<Claim> claims, Profile? onBehalfOf)
	{
		if (!_authz.Federate(claims, inbox))
			return;
		await _bus.Publish(FormatMessage(inbox, activity, onBehalfOf));
		_logger.LogInformation("Scheduled message type {Activity} for delivery to {Inbox}",
			activity.GetType(), inbox);
		_logger.LogDebug("Scheduled message type {Activity} from ({Thread})",
			activity.GetType(), Environment.CurrentManagedThreadId);
	}

	/// <inheritdoc />
	public async Task Publish(Uri inbox, Post post, Profile onBehalfOf, IEnumerable<Claim> claims, Mention? extraMention = default)
	{
		var doc = PopulateMentions(post, extraMention);
		var activity = _document.Create(onBehalfOf, doc);
		await Deliver(inbox, activity, claims, onBehalfOf);
	}

	/// <inheritdoc />
	public async Task Update(Uri inbox, Post post, Profile onBehalfOf, IEnumerable<Claim> claims, Mention? extraMention = default)
	{
		var doc = PopulateMentions(post, extraMention);
		var activity = _document.Update(onBehalfOf, doc);
		await Deliver(inbox, activity, claims, onBehalfOf);
	}

	/// <inheritdoc />
	public async Task Delete(Uri inbox, Post post, IEnumerable<Claim> claims, Profile onBehalfOf)
	{
		var activity = _document.Delete(onBehalfOf, post.FediId);
		await Deliver(inbox, activity, claims, onBehalfOf);
	}

	/// <inheritdoc />
	public async Task Share(Uri inbox, Post post, IEnumerable<Claim> claims, Profile onBehalfOf)
	{
		var activity = _document.Announce(onBehalfOf, post.FediId);
		await Deliver(inbox, activity, claims, onBehalfOf);
	}

	/// <inheritdoc />
	public async Task Like(Uri inbox, Post post, IEnumerable<Claim> claims, Profile onBehalfOf)
	{
		var activity = _document.Like(onBehalfOf, post.FediId);
		await Deliver(inbox, activity, claims, onBehalfOf);
	}

	/// <inheritdoc />
	public async Task Follow(Uri inbox, Profile target, IEnumerable<Claim> claims, Profile actor)
	{
		var document = _document.Follow(actor, target, implicitId: true);
		await Deliver(inbox, document, claims, actor);
	}

	/// <inheritdoc />
	public async Task Unfollow(Uri inbox, Profile target, IEnumerable<Claim> claims, Profile actor)
	{
		var document = _document.Undo(actor, _document.Follow(actor, target, implicitId: true));
		await Deliver(inbox, document, claims, actor);
	}

	/// <inheritdoc />
	public async Task RemoveFollower(Uri inbox, Profile target, IEnumerable<Claim> claims, Profile actor) => await RejectFollower(inbox, target, claims, actor);

	/// <inheritdoc />
	public async Task AcceptFollower(Uri inbox, Profile target, IEnumerable<Claim> claims, Profile actor)
	{
		var document = _document.Accept(actor, _document.Follow(target, actor));
		await Deliver(inbox, document, claims, actor);
	}

	/// <inheritdoc />
	public async Task RejectFollower(Uri inbox, Profile target, IEnumerable<Claim> claims, Profile actor)
	{
		var document = _document.Reject(actor, _document.Follow(target, actor));
		await Deliver(inbox, document, claims, actor);
	}

	public async Task PendingFollower(Uri inbox, Profile target, Profile actor, IEnumerable<Claim> claims)
	{
		var document = _document.TentativeAccept(actor, _document.Follow(target, actor));
		await Deliver(inbox, document, claims, actor);
	}

	/// <inheritdoc />
	public async Task Report(Uri inbox, ModerationReport report, IEnumerable<Claim> claims, ModerationReport.Scope scope)
	{
		var systemActor = Profile.GetModeratorsProfile();
		switch (scope)
		{
			case ModerationReport.Scope.Profile:
				foreach (var subject in report.Subjects)
				{
					await Deliver(inbox, _document.Flag(systemActor!, inbox, report, scope, subject), claims, systemActor);
				}
				break;
			case ModerationReport.Scope.Domain:
			case ModerationReport.Scope.Full:
				await Deliver(inbox, _document.Flag(systemActor!, inbox, report, scope), claims, systemActor);
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(scope), scope, null);
		}

	}

	private ASObject PopulateMentions(Post post, Mention? extraMention)
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

		return doc;
	}


	private ActivityMessage FormatMessage(Uri inbox, ASType activity, Profile? onBehalfOf)
	{
		var act = activity.Is<ASObject>(out var o)
			? o.Id
			: activity.Is<ASLink>(out var l)
				? l.HRef.ToString()
				: null;
		act ??= string.Join(',', activity.TypeMap.ASTypes);
		var data = activity switch
		{
			FlagActivity f => PreprocessFlag(f),
			_ => _document.Serialize(activity)
		};
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

		// Pleroma and friends don't parse Flag activities properly if the object is a single value
		// So, we need to massage it into a single element array
		// See https://activitypub.software/TransFem-org/Sharkey/-/merge_requests/690
		// To my knowledge, everyone parses the array correctly
		string PreprocessFlag(FlagActivity flag)
		{
			if (flag.Object.Count > 1)
				return _document.Serialize(flag);

			var node = _document.SerializeToNode(flag);
			if (node!["object"] is not { } obj)
			{
				_logger.LogDebug("Failed to preprocess FlagActivity {Activity}", JsonSerializer.Serialize(node));
				_logger.LogInformation("Failed to preprocess FlagActivity {Id}; falling back to default serialization", flag.Id);
				return _document.Serialize(flag);
			}

			if (!obj.AsValue().TryGetValue<string>(out var objVal))
			{
				_logger.LogDebug("Failed to preprocess FlagActivity {Activity}", JsonSerializer.Serialize(node));
				_logger.LogInformation("Failed to preprocess FlagActivity {Id}; falling back to default serialization", flag.Id);
				return _document.Serialize(flag);
			}

			node["object"] = new JsonArray(objVal);
			return _document.Serialize(node);
		}
	}
}