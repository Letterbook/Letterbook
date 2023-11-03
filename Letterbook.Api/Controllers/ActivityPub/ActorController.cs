using Letterbook.Api.Dto;
using Letterbook.Core;
using Letterbook.Core.Exceptions;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models;
using Letterbook.Core.Values;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Letterbook.Api.Controllers.ActivityPub;

/// <summary>
/// Provides the endpoints specified for Actors in the ActivityPub spec
/// https://www.w3.org/TR/activitypub/#actors
/// </summary>
[ApiController]
[Route("[controller]")]
[AcceptHeader("application/ld+json",
    "application/ld+json; profile=\"https://www.w3.org/ns/activitystreams\"", "application/activity+json")]
public class ActorController : ControllerBase
{
    private readonly SnakeCaseRouteTransformer _transformer = new();
    private readonly Uri _baseUri;
    private readonly ILogger<ActorController> _logger;
    private readonly IActivityService _activityService;
    private readonly IProfileService _profileService;

    public ActorController(IOptions<CoreOptions> config, ILogger<ActorController> logger,
        IActivityService activityService, IProfileService profileService)
    {
        _baseUri = new Uri($"{config.Value.Scheme}://{config.Value.DomainName}");
        _logger = logger;
        _activityService = activityService;
        _profileService = profileService;
        _logger.LogInformation("Loaded ActorController");
    }


    [HttpGet]
    [Route("{id}")]
    public ActionResult<AsAp.Actor> GetActor(int id)
    {
        throw new NotImplementedException();
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
    [ProducesResponseType(typeof (AsAp.Activity), StatusCodes.Status200OK, "application/ld+json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    [ProducesResponseType(StatusCodes.Status421MisdirectedRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> PostInbox(string id, AsAp.Activity activity)
    {
        var localId = ShortId.ToGuid(id);
        var activityType = Enum.Parse<ActivityType>(activity.Type);
        try
        {
            switch (activityType)
            {
                case ActivityType.Accept:
                    break;
                case ActivityType.Add:
                    break;
                case ActivityType.Announce:
                    break;
                case ActivityType.Block:
                    break;
                case ActivityType.Create:
                    break;
                case ActivityType.Delete:
                    break;
                case ActivityType.Dislike:
                    break;
                case ActivityType.Flag:
                    break;
                case ActivityType.Follow:
                    return await InboxFollow(localId, activity);
                case ActivityType.Like:
                    break;
                case ActivityType.Move:
                    break;
                case ActivityType.Undo:
                    return await InboxUndo(localId, activity);
                case ActivityType.Update:
                    break;
                case ActivityType.Question:
                    break;
                case ActivityType.Reject:
                    break;
                case ActivityType.Remove:
                    break;
                case ActivityType.TentativeReject:
                    break;
                case ActivityType.TentativeAccept:
                    break;
                // Some of these activities may be worth understanding eventually, even if letterbook will likely never
                // produce them.
                // Esp listen, read, travel, and view
                case ActivityType.Arrive:
                case ActivityType.Ignore:
                case ActivityType.Invite:
                case ActivityType.Join:
                case ActivityType.Leave:
                case ActivityType.Listen:
                case ActivityType.Offer:
                case ActivityType.Read:
                case ActivityType.Travel:
                case ActivityType.View:
                    _logger.LogInformation("Ignored unhandled activity {ActivityType}", activityType);
                    _logger.LogDebug("Ignored unhandled activity details {@Activity}", activity);
                    return Accepted();
                case ActivityType.Unknown:
                default:
                    _logger.LogWarning("Ignored unknown activity {ActivityType}", activityType);
                    _logger.LogDebug("Ignored unknown activity details {@Activity}", activity);
                    return Accepted(); // ?
            }
        }
        catch (CoreException e)
        {
            if (e.Flagged(ErrorCodes.WrongAuthority)) return StatusCode(422, new ErrorMessage(e));
            if (e.Flagged(ErrorCodes.InvalidRequest)) return UnprocessableEntity(new ErrorMessage(e));
            if (e.Flagged(ErrorCodes.MissingData)) return NotFound(new ErrorMessage(e));
            if (e.Flagged(ErrorCodes.DuplicateEntry)) return Conflict(new ErrorMessage(e));
            throw;
        }
        throw new NotImplementedException();
    }

    [HttpPost]
    [Route("[action]")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<ActionResult> SharedInbox(AsAp.Activity activity)
    {
        throw new NotImplementedException();
        // await _activityService.ReceiveNotes(new Note[] { }, Enum.Parse<ActivityType>(activity.Type), null);
        return Accepted();
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
    
    private async Task<IActionResult> InboxUndo(Guid id, AsAp.Activity activity)
    {
        if (activity.Object.Count > 1)
            return new BadRequestObjectResult(new ErrorMessage(
                ErrorCodes.None.With((int)ActivityPubErrorCodes.UnknownSemantics), "Cannot Undo multiple Activities"));
        if (activity.Object.SingleOrDefault() is not AsAp.Activity subject)
            return new BadRequestObjectResult(new ErrorMessage(
                ErrorCodes.None.With((int)ActivityPubErrorCodes.UnknownSemantics), "Object of an Undo must be another Activity"));
        var activityType = Enum.Parse<ActivityType>(subject.Type);
        switch (activityType)
        {
            case ActivityType.Announce:
                throw new NotImplementedException();
            case ActivityType.Block:
                throw new NotImplementedException();
            case ActivityType.Follow:
                if ((activity.Actor.SingleOrDefault() ?? subject.Actor.SingleOrDefault()) is not AsAp.Actor actor)
                    return new BadRequestObjectResult(new ErrorMessage(
                        ErrorCodes.None.With((int)ActivityPubErrorCodes.UnknownSemantics),
                        "Exactly one Actor can unfollow at a time"));
                if (actor.Id is null)
                    return new BadRequestObjectResult(new ErrorMessage(ErrorCodes.InvalidRequest,
                        "Actor ID is required to unfollow"));
                
                await _profileService.RemoveFollower(id, actor.Id);
                
                return new OkResult();
            case ActivityType.Like:
                throw new NotImplementedException();
            default:
                _logger.LogInformation("Ignored unknown Undo target {ActivityType}", activityType);
                return new AcceptedResult();
        }
    }

    private async Task<IActionResult> InboxFollow(Guid localId, AsAp.Activity activity)
    {
        if (activity.Actor.Count > 1) return BadRequest(new ErrorMessage(ErrorCodes.None, "Only one Actor can follow at a time"));
        var actor = activity.Actor.FirstOrDefault();
        if (actor?.Id is null) return BadRequest(new ErrorMessage(ErrorCodes.None, "Actor ID is required for follower"));
                    
        var state = await _profileService.ReceiveFollowRequest(localId, actor.Id);
                    
        return state switch
        {
            FollowState.Accepted => Ok(ActivityResponse.AcceptActivity("Follow", activity.Id)), // Accept(Follow)
            FollowState.Pending => Ok(ActivityResponse.TentativeAcceptActivity("Follow", activity.Id)), // PendingAccept(Follow)
            _ => Ok(ActivityResponse.RejectActivity("Follow", activity.Id)), // Reject(Follow)
        };
    }
}