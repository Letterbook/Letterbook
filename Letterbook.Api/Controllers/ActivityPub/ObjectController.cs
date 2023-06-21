using Microsoft.AspNetCore.Mvc;

namespace Letterbook.Api.Controllers.ActivityPub;

[ApiController]
[Route("[controller]/{type}")]
public class ObjectController
{
    [HttpGet]
    [Route("{id}/collections/[action]")]
    public IActionResult Likes(int id, string type)
    {
        throw new NotImplementedException();
    }
    
    [HttpGet]
    [Route("{id}/collections/[action]")]
    public IActionResult Shares(int id, string type)
    {
        throw new NotImplementedException();
    }
}