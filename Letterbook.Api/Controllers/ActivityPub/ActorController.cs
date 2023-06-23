using System.Reflection;
using Letterbook.Api.Extensions;
using Microsoft.AspNetCore.Mvc;
using Fedodo.NuGet.ActivityPub;
using Fedodo.NuGet.ActivityPub.Model.ActorTypes;
using Fedodo.NuGet.ActivityPub.Model.ActorTypes.SubTypes;

namespace Letterbook.Api.Controllers.ActivityPub;

/// <summary>
/// Provides the endpoints specified for Actors in the ActivityPub spec
/// https://www.w3.org/TR/activitypub/#actors
/// </summary>
[ApiController]
[Route("[controller]")]
public class ActorController
{
    private readonly SnakeCaseRouteTransformer _transformer = new();
    private readonly Uri _baseUri;

    public ActorController(Uri baseUri)
    {
        _baseUri = baseUri;
    }
    
    
    [HttpGet]
    [Route("{id}")]
    public IActionResult GetActor(int id)
    {
        var actor = new Actor
        {
            Inbox = collectionUri(ActionName(nameof(GetInbox)), id.ToString()),
            Outbox = collectionUri(ActionName(nameof(GetOutbox)), id.ToString()),
            Endpoints = new Endpoints
            {
                SharedInbox = collectionUri(ActionName(nameof(SharedInbox)), id.ToString())
            },
            Followers = collectionUri(ActionName(nameof(GetFollowers)), id.ToString()),
            Following = collectionUri(ActionName(nameof(GetFollowing)), id.ToString()),
            // Fedodo doesn't seem to have Liked on Actor? Shrug.
        };
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
    public IActionResult PostInbox(int id)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    [Route("[action]")]
    public IActionResult SharedInbox()
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

    private Uri collectionUri(string action, string id)
    {
        var route = "/actor/" + RouteTemplate(action)
            .Replace("[action]", ActionName(action))
            .Replace("{id}", id);
        var result = new Uri(_baseUri, _transformer.TransformOutbound(route));
        
        return result;
    }

    private static string ActionName(string action)
    {
        var method = typeof(ActorController)
            .GetRuntimeMethod(action, Array.Empty<Type>());
        var actionName = method.GetCustomAttribute<ActionNameAttribute>();
        return actionName?.Name;
    }

    private static string RouteTemplate(string action)
    {
        var method = typeof(ActorController)
            .GetRuntimeMethod(action, Array.Empty<Type>());
        var route = method.GetCustomAttribute<RouteAttribute>();
        return route?.Template;
    }
}