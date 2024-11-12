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
using Letterbook.Core.Models;
using Letterbook.Core.Values;
using Medo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
	private readonly IPostService _postService;
	private readonly IApCrawlerScheduler _apCrawler;
	private static readonly IMapper ActorMapper = new Mapper(AstMapper.Profile);
	private static readonly IMapper Mapper = new Mapper(AstMapper.Default);

	public ActorController(ILogger<ActorController> logger,
		IProfileService profileService, IPostService postService, IApCrawlerScheduler apCrawler)
	{
		_logger = logger;
		_profileService = profileService;
		_postService = postService;
		_apCrawler = apCrawler;
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
		var actor = ActorMapper.Map<ProfileActor>(profile);

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
	public async Task<IActionResult> PostInbox(ProfileId id, ASType activity)
	{
		try
		{
			if (activity.Is<AcceptActivity>(out var accept))
			{
				_logger.LogDebug("Inbox received: {Activity}", "Accept");
				return await InboxAccept(id, accept);
			}
			if (activity.Is<RejectActivity>(out var reject))
			{
				return await InboxReject(id, reject);
			}
			if (activity.Is<FollowActivity>(out var follow))
			{
				_logger.LogDebug("Inbox received: {Activity}", "Follow");
				return await InboxFollow(id, follow);
			}
			if (activity.Is<UndoActivity>(out var undo))
			{
				_logger.LogDebug("Inbox received: {Activity}", "Undo");
				return await InboxUndo(id, undo);
			}

			if (activity.Is<CreateActivity>(out var create))
			{
				return await InboxCreate(id, create);
			}

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
		if (wrapper.Object.SingleOrDefault()?.Value?.Is<ASActivity>(out var asObject) != true)
		{
			_logger.LogDebug("Can't unwrap; Not an activity");
			error = new BadRequestObjectResult(new ErrorMessage(ErrorCodes.UnknownSemantics,
				$"Object of {verb} must have exactly one value, which must be another Activity"));
			activity = default;
			actorId = default;
			return false;
		}

		if (asObject!.Actor.SingleOrDefault() is not { } actor)
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

	private async Task<IActionResult> InboxCreate(ProfileId id, CreateActivity activity)
	{
		var posts = activity.Object.ValueItems.Select(Mapper.Map<Post>);
		var inBand = true;
		foreach (var post in posts)
		{
			if (post.Contents.Count == 0)
			{
				_logger.LogWarning("Create: object {Id} does not appear to be a Post", post.FediId);
				continue;
			}

			var created = await _postService.As(User.Claims).ReceiveCreate(post);
			if (created == null) inBand = false;
		}

		if (activity.Object.LinkItems.Any()) inBand = false;
		foreach (var link in activity.Object.LinkItems)
		{
			await _apCrawler.CrawlPost(id, link.HRef);
		}

		return inBand ? Ok() : Accepted();
	}

	private async Task<IActionResult> InboxAccept(ProfileId localId, ASActivity activity)
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

	private async Task<IActionResult> InboxReject(ProfileId localId, RejectActivity rejectActivity)
	{
		_logger.LogDebug("Inbox received: {Activity}", "Reject");
		if (!TryUnwrapActivity(rejectActivity, "Reject", out var activityObject, out var actorId, out var error))
			return error;

		if (activityObject.Is<FollowActivity>(out var followActivity))
		{
			if (actorId.ToString().Contains(localId.ToString())
				&& rejectActivity.Actor.SingleOrDefault()?.TryGetId(out var targetId) == true)
			{
				await _profileService.As(User.Claims).ReceiveFollowReply(localId, targetId, FollowState.Rejected);
				return Ok();
			}

			return BadRequest();
		}

		_logger.LogWarning("{Method}: Unknown object semantics {@ObjectType}", nameof(InboxReject) , activityObject.TypeMap.ASTypes);
		return Accepted();
	}

	private async Task<IActionResult> InboxUndo(ProfileId id, ASActivity activity)
	{
		if (!TryUnwrapActivity(activity, "Undo", out var activityObject, out var undoActorId, out var error))
			return error;

		if (activityObject.Is<AnnounceActivity>(out var announceActivity))
			throw new NotImplementedException();
		if (activityObject.Is<BlockActivity>(out var blockActivity))
			throw new NotImplementedException();
		if (activityObject.Is<FollowActivity>(out var followActivity))
		{
			_logger.LogDebug("Undo activity: {Object}", "Follow");
			if (followActivity.Object.SingleOrDefault() is not {} target
			    || !target.TryGetId(out var targetId)
			    || !targetId.ToString().Contains(id.ToString()))
				return BadRequest();
			if (followActivity.Actor.SingleOrDefault() is not {} actor
				|| !actor.TryGetId(out var followerId))
				return BadRequest();

			await _profileService.As(User.Claims).RemoveFollower(id, followerId);
			return Ok();
		}
		if (activityObject.Is<LikeActivity>(out var likeActivity))
			throw new NotImplementedException();

		_logger.LogInformation("Ignored unknown Undo target {ActivityType}", activityObject.TypeMap.ASTypes);
		return new AcceptedResult();
	}

	private async Task<IActionResult> InboxFollow(ProfileId localId, ASActivity followRequest)
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