using Microsoft.AspNetCore.DataProtection.Internal;
using Microsoft.AspNetCore.Mvc;

namespace Letterbook.Api.Controllers.ActivityPub;

[ApiController]
[Route("[controller]")]
public class ActorController
{
    [HttpGet]
    [Route("{id}")]
    public IActionResult GetActor(int id)
    {
        throw new NotImplementedException();
    }
    
    [HttpGet]
    [Route("{id}/collections/[action]")]
    public IActionResult GetFollowers(int id)
    {
        throw new NotImplementedException();
    }
    
    [HttpGet]
    [Route("{id}/collections/[action]")]
    public IActionResult GetFollowing(int id)
    {
        throw new NotImplementedException();
    }
    
    [HttpGet]
    [Route("{id}/collections/[action]")]
    public IActionResult GetLiked(int id)
    {
        throw new NotImplementedException();
    }
    
    [HttpGet]
    [Route("{id}/inbox")]
    public IActionResult GetInbox(int id)
    {
        throw new NotImplementedException();
    }
    
    [HttpPost]
    [Route("{id}/inbox")]
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
    [Route("{id}/outbox")]
    public IActionResult GetOutbox(int id)
    {
        throw new NotImplementedException();
    }
    
    [HttpPost]
    [Route("{id}/outbox")]
    public IActionResult PostOutbox(int id)
    {
        throw new NotImplementedException();
    }
}