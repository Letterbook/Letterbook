using System.Reflection;
using Letterbook.Core;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models;
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
    }


    [HttpGet]
    [Route("{id}")]
    public ActionResult<DTO.Actor> GetActor(int id)
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    [ProducesResponseType(StatusCodes.Status421MisdirectedRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public IActionResult PostInbox(string id, DTO.Activity activity)
    {
        var localId = ShortId.ToGuid(id);
        var activityType = Enum.Parse<ActivityType>(activity.Type);
        switch (activityType)
        {
            case ActivityType.Accept:
                // Unwrap and reparse
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
            case ActivityType.Like:
            case ActivityType.Dislike:
                break;
            case ActivityType.Flag:
                break;
            case ActivityType.Follow:
                var actorIds = activity.Actor.Select(actor => actor.Id);
                if (actorIds.Any(actorId => actorId == null)) return BadRequest();
                foreach (var actor in actorIds)
                {
                    _profileService.ReceiveFollowRequest(localId, actor!);
                }
                return Ok();
            case ActivityType.Undo:
                // Unwrap and reparse
                break;
            case ActivityType.Update:
                break;
            case ActivityType.Question:
                break;
            case ActivityType.Reject:
                // Unwrap and reparse
                break;
            case ActivityType.Remove:
                break;
            case ActivityType.TentativeReject:
                // Unwrap and reparse
                break;
            case ActivityType.TentativeAccept:
                // Unwrap and reparse
                break;
            case ActivityType.Arrive:
            case ActivityType.Ignore:
            case ActivityType.Invite:
            case ActivityType.Join:
            case ActivityType.Leave:
            case ActivityType.Listen:
            case ActivityType.Move:
            case ActivityType.Offer:
            case ActivityType.Read:
            case ActivityType.Travel:
            case ActivityType.View:
                // Some of these activities may be worth understanding eventually, even if letterbook will likely never
                // produce them.
                // Esp listen, read, travel, and view
                _logger.LogInformation("Ignored unhandled activity {ActivityType}", activityType);
                _logger.LogDebug("Ignored unhandled activity details {@Activity}", activity);
                return Accepted();
            case ActivityType.Unknown:
            default:
                _logger.LogWarning("Ignored unknown activity {ActivityType}", activityType);
                _logger.LogDebug("Ignored unknown activity details {@Activity}", activity);
                return Accepted(); // ?
        }
        throw new NotImplementedException();
    }

    [HttpPost]
    [Route("[action]")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<ActionResult> SharedInbox(DTO.Activity activity)
    {
        await _activityService.ReceiveNotes(new Note[]{}, Enum.Parse<ActivityType>(activity.Type), null);
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

    private Uri CollectionUri(string actionName, string id)
    {
        var (action, routeTemplate) = ActionAttributes(actionName);
        var route = "/actor/" + routeTemplate
            .Replace("[action]", action)
            .Replace("{id}", id);
        var transformed = string.Join("/", route
            .Split("/")
            .Select(part => _transformer.TransformOutbound(part)));
        var result = new Uri(_baseUri, transformed);

        return result;
    }

    private static (string action, string route) ActionAttributes(string action)
    {
        var method = typeof(ActorController).GetMethod(action);

        var actionName = (method ?? throw new InvalidOperationException($"no method with name {action}"))
            .GetCustomAttribute<ActionNameAttribute>();
        var route = method.GetCustomAttribute<RouteAttribute>();
        if (route == null) throw new InvalidOperationException($"no route for action {action}");
        return (actionName?.Name ?? method.Name, route.Template);
    }
}