using System.Diagnostics.CodeAnalysis;
using ActivityPub.Types.AS;
using ActivityPub.Types.AS.Extended.Activity;
using AutoMapper;
using Letterbook.Adapter.ActivityPub;
using Letterbook.Adapter.ActivityPub.Mappers;
using Letterbook.Adapter.ActivityPub.Types;
using Letterbook.Api.Dto;
using Letterbook.Api.Swagger;
using Letterbook.Core;
using Letterbook.Core.Adapters;
using Letterbook.Core.Exceptions;
using Letterbook.Core.Extensions;
using Letterbook.Core.Values;
using Medo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Letterbook.Api.Controllers.ActivityPub;

/// <summary>
/// Provides the endpoints specified for Actors in the ActivityPub spec
/// https://www.w3.org/TR/activitypub/#actors
/// </summary>
[ApiExplorerSettings(GroupName = Docs.ActivityPubV1)]
[ApiController]
[Route("[controller]")]
[Consumes("application/ld+json",
	"application/ld+json; profile=\"https://www.w3.org/ns/activitystreams\"",
	"application/activity+json")]
// [Produces("application/ld+json",
	// "application/ld+json; profile=\"https://www.w3.org/ns/activitystreams\"",
	// "application/activity+json")]
[Authorize(policy: Constants.ActivityPubPolicy)]
public class ActorController : ControllerBase
{
	private readonly ILogger<ActorController> _logger;
	private readonly IProfileService _profileService;
	private readonly IActivityMessagePublisher _messagePublisher;
	private readonly IActivityPubDocument _apDoc;
	private static readonly IMapper ActorMapper = new Mapper(AstMapper.Default);

	public ActorController(IOptions<CoreOptions> config, ILogger<ActorController> logger,
		IProfileService profileService, IActivityMessagePublisher messagePublisher, IActivityPubDocument apDoc)
	{
		_logger = logger;
		_profileService = profileService;
		_messagePublisher = messagePublisher;
		_apDoc = apDoc;
		_logger.LogInformation("Loaded {Controller}", nameof(ActorController));
	}


	[HttpGet]
	[Route("{id}")]
	[AllowAnonymous]
	public async Task<IActionResult> GetActor(string id)
	{
		if (!Id.TryAsUuid7(id, out var uuid))
			return BadRequest();

		var profile = await _profileService.As(User.Claims).LookupProfile(uuid);
		if (profile == null) return NotFound();
		var actor = ActorMapper.Map<PersonActorExtension>(profile);

		return Ok(actor);
	}

	[HttpGet]
	[ActionName("Followers")]
	[Route("{id}/collections/[action]")]
	public IActionResult GetFollowers(Uuid7 id)
	{
		return NoContent();
	}

	[HttpGet]
	[ActionName("Following")]
	[Route("{id}/collections/[action]")]
	public IActionResult GetFollowing(Uuid7 id)
	{
		return NoContent();
	}

	[HttpGet]
	[ActionName("Liked")]
	[Route("{id}/collections/[action]")]
	public IActionResult GetLiked(Uuid7 id)
	{
		return NoContent();
	}

	[HttpGet]
	[ActionName("Inbox")]
	[Route("{id}/[action]")]
	public IActionResult GetInbox(Uuid7 id)
	{
		return NoContent();
	}

	[HttpPost]
	[ActionName("Inbox")]
	[Route("{id}/[action]")]
	[ProducesResponseType(typeof(ASActivity), StatusCodes.Status200OK, "application/ld+json")]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status202Accepted)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status410Gone)]
	[ProducesResponseType(StatusCodes.Status421MisdirectedRequest)]
	[ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
	public async Task<IActionResult> PostInbox(Uuid7 id, ASType activity)
	{
		try
		{
			if (activity.Is<AcceptActivity>(out var accept))
				return await InboxAccept(id, accept);
			if (activity.Is<RejectActivity>(out var reject))
				return await InboxReject(id, reject);
			if (activity.Is<FollowActivity>(out var follow))
				return await InboxFollow(id, follow);
			if (activity.Is<UndoActivity>(out var undo))
				return await InboxUndo(id, undo);

			_logger.LogWarning("Ignored unknown activity {ActivityType}", activity.GetType());
			_logger.LogDebug("Ignored unknown activity details {@Activity}", activity);
			return Accepted();
		}
		catch (CoreException e)
		{
			if (e.Flagged(ErrorCodes.WrongAuthority)) return StatusCode(421, new ErrorMessage(e));
			if (e.Flagged(ErrorCodes.InvalidRequest)) return UnprocessableEntity(new ErrorMessage(e));
			if (e.Flagged(ErrorCodes.MissingData)) return NotFound(new ErrorMessage(e));
			if (e.Flagged(ErrorCodes.DuplicateEntry)) return Conflict(new ErrorMessage(e));
			throw;
		}
	}

	[HttpPost]
	[Route("[action]")]
	[ProducesResponseType(StatusCodes.Status202Accepted)]
	public IActionResult SharedInbox(ASType activity)
	{
		return NoContent();
	}

	[HttpGet]
	[ActionName("Outbox")]
	[Route("{id}/[action]")]
	public IActionResult GetOutbox(Uuid7 id)
	{
		return NoContent();
	}

	[HttpPost]
	[ActionName("Outbox")]
	[Route("{id}/[action]")]
	public IActionResult PostOutbox(Uuid7 id)
	{
		throw new NotImplementedException();
	}

	/* * * * * * * * * * * * *
     * Support methods       *
     * * * * * * * * * * * * */

	private bool TryUnwrapActivity(ASActivity wrapper, string verb, [NotNullWhen(true)] out ASActivity? activity,
		[NotNullWhen(true)] out Uri? actorId, [NotNullWhen(false)] out IActionResult? error)
	{
		if (wrapper.Object.SingleOrDefault()?.Value is not ASActivity asObject)
		{
			_logger.LogDebug("Can't unwrap; Not an activity");
			error = new BadRequestObjectResult(new ErrorMessage(ErrorCodes.UnknownSemantics,
				$"Object of {verb} must have exactly one value, which must be another Activity"));
			activity = default;
			actorId = default;
			return false;
		}

		if (asObject.Actor.SingleOrDefault() is not { } actor)
		{
			_logger.LogDebug("Can't unwrap; invalid actor");
			error = new BadRequestObjectResult(new ErrorMessage(ErrorCodes.UnknownSemantics,
				$"{verb} must be performed by exactly one Actor"));
			activity = default;
			actorId = default;
			return false;
		}

		if (!actor.TryGetId(out var id))
		{
			_logger.LogDebug("Can't unwrap; no actor ID");
			error = new BadRequestObjectResult(new ErrorMessage(ErrorCodes.InvalidRequest,
				"Actor ID is required"));
			activity = default;
			actorId = default;
			return false;
		}

		activity = asObject;
		actorId = id;
		error = default;
		return true;
	}

	private async Task<IActionResult> InboxAccept(Uuid7 localId, ASActivity activity)
	{
		if (!TryUnwrapActivity(activity, "Accept", out var activityObject, out var actorId, out var error))
			return error;

		if (activityObject.Is<FollowActivity>())
		{
			await _profileService.As(User.Claims).ReceiveFollowReply(localId, actorId, FollowState.Accepted);
			return Ok();
		}

		_logger.LogWarning("{Method}: Unknown object semantics {@ObjectType}", nameof(InboxAccept) , activityObject.TypeMap.ASTypes);
		return Accepted();
	}

	private async Task<IActionResult> InboxReject(Uuid7 localId, RejectActivity activity)
	{
		if (!TryUnwrapActivity(activity, "Reject", out var activityObject, out var actorId, out var error))
			return error;

		if (activityObject.Is<FollowActivity>())
		{
			await _profileService.As(User.Claims).ReceiveFollowReply(localId, actorId, FollowState.Rejected);
			return Ok();
		}

		_logger.LogWarning("{Method}: Unknown object semantics {@ObjectType}", nameof(InboxReject) , activityObject.TypeMap.ASTypes);
		return Accepted();
	}

	private async Task<IActionResult> InboxUndo(Uuid7 id, ASActivity activity)
	{
		if (activity.Object.SingleOrDefault()?.Value is not ASActivity activityObject)
			return new BadRequestObjectResult(new ErrorMessage(ErrorCodes.UnknownSemantics,
				"Object of an Undo must have exactly one value, which must be another Activity"));
		if (activityObject.Is<AnnounceActivity>(out var announceActivity))
			throw new NotImplementedException();
		if (activityObject.Is<BlockActivity>(out var blockActivity))
			throw new NotImplementedException();
		if (activityObject.Is<FollowActivity>(out var followActivity))
		{
			if ((followActivity.Actor.SingleOrDefault() ?? activityObject.Actor.SingleOrDefault()) is not { } actor)
				return new BadRequestObjectResult(new ErrorMessage(ErrorCodes.UnknownSemantics,
					"Undo:Follow can only be performed by exactly one Actor at a time"));
			if (!actor.TryGetId(out var actorId))
				return new BadRequestObjectResult(new ErrorMessage(ErrorCodes.InvalidRequest,
					"Actor ID is required to Undo:Follow"));
			await _profileService.As(User.Claims).RemoveFollower(id, actorId);
			return new OkResult();
		}
		if (activityObject.Is<LikeActivity>(out var likeActivity))
			throw new NotImplementedException();

		_logger.LogInformation("Ignored unknown Undo target {ActivityType}", activityObject.TypeMap.ASTypes);
		return new AcceptedResult();
	}

	private async Task<IActionResult> InboxFollow(Uuid7 localId, ASActivity followRequest)
	{
		if (followRequest.Actor.Count > 1) return BadRequest(new ErrorMessage(ErrorCodes.None, "Only one Actor can follow at a time"));
		var actor = followRequest.Actor.First();
		if (!actor.TryGetId(out var actorId))
			return BadRequest(new ErrorMessage(ErrorCodes.None, "Actor ID is required for follower"));

		followRequest.TryGetId(out var activityId);
		await _profileService.As(User.Claims).ReceiveFollowRequest(localId, actorId, activityId);

		return Accepted();
	}
}