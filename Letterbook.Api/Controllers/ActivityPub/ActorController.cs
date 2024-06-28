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
using Letterbook.Core.Events;
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
[Authorize(policy: "ActivityPub")]
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
	public IActionResult GetFollowers(int id)
	{
		throw new NotImplementedException();
	}

	[HttpGet]
	[ActionName("Following")]
	[Route("{id}/collections/[action]")]
	public IActionResult GetFollowing(int id)
	{
		throw new NotImplementedException();
	}

	[HttpGet]
	[ActionName("Liked")]
	[Route("{id}/collections/[action]")]
	public IActionResult GetLiked(int id)
	{
		throw new NotImplementedException();
	}

	[HttpGet]
	[ActionName("Inbox")]
	[Route("{id}/[action]")]
	public IActionResult GetInbox(int id)
	{
		throw new NotImplementedException();
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
	public async Task<IActionResult> PostInbox(string id, ASType activity)
	{
		var localId = Uuid7.FromId25String(id);
		try
		{
			if (activity.Is<AcceptActivity>(out var accept))
				return await InboxAccept(localId, accept);
			if (activity.Is<FollowActivity>(out var follow))
				return await InboxFollow(localId, follow);
			if (activity.Is<UndoActivity>(out var undo))
				return await InboxUndo(localId, undo);

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
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task<ActionResult> SharedInbox(ASType activity)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
		throw new NotImplementedException();
	}

	[HttpGet]
	[ActionName("Outbox")]
	[Route("{id}/[action]")]
	public IActionResult GetOutbox(int id)
	{
		throw new NotImplementedException();
	}

	[HttpPost]
	[ActionName("Outbox")]
	[Route("{id}/[action]")]
	public IActionResult PostOutbox(int id)
	{
		throw new NotImplementedException();
	}

	/* * * * * * * * * * * * *
     * Support methods       *
     * * * * * * * * * * * * */

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	private async Task<IActionResult> InboxAccept(Guid localId, ASActivity activity)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
		throw new NotImplementedException();
	}

	private async Task<IActionResult> InboxUndo(Guid id, ASActivity activity)
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

	private async Task<IActionResult> InboxFollow(Guid localId, ASActivity followRequest)
	{
		if (followRequest.Actor.Count > 1) return BadRequest(new ErrorMessage(ErrorCodes.None, "Only one Actor can follow at a time"));
		var actor = followRequest.Actor.First();
		if (!actor.TryGetId(out var actorId))
			return BadRequest(new ErrorMessage(ErrorCodes.None, "Actor ID is required for follower"));

		followRequest.TryGetId(out var activityId);
		var relation = await _profileService.As(User.Claims).ReceiveFollowRequest(localId, actorId, activityId);

		ASType resultActivity = relation.State switch
		{
			FollowState.Accepted => _apDoc.Accept(relation.Follows, followRequest),
			FollowState.Pending => _apDoc.TentativeAccept(relation.Follows, followRequest),
			_ => _apDoc.Reject(relation.Follows, followRequest)
		};

		// We should publish and implement a reply negotiation mechanism. The options are basically response or inbox.
		// Response would mean reply "in-band" via the http response.
		// Inbox would mean reply "out-of-band" via a new POST to the actor's inbox.
		// But, that doesn't exist, yet. The if(false) is so we don't forget.
		var acceptResponse = false;
		if (acceptResponse) return Ok(resultActivity);
		await _messagePublisher.Deliver(relation.Follower.Inbox, resultActivity, relation.Follows);
		return Accepted();
	}
}