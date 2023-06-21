using Microsoft.AspNetCore.Mvc;

namespace Letterbook.Api.Controllers.ActivityPub;

[ApiController]
[Route("[controller]/{type}")]
public class ObjectController
{
    [HttpGet]
    [Route("{id}/collections/[action]")]
    public IActionResult GetLikes(int id, string type)
    {
        throw new NotImplementedException();
    }
    
    [HttpGet]
    [Route("{id}/collections/[action]")]
    public IActionResult GetShares(int id, string type)
    {
        throw new NotImplementedException();
    }
}