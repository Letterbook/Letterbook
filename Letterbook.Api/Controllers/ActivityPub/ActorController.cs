using Microsoft.AspNetCore.Mvc;

namespace Letterbook.Api.Controllers.ActivityPub;

[ApiController]
[Route("[controller]")]
public class ActorController
{
    [HttpGet]
    [Route("{id}/[action]")]
    public void GetActor(int id)
    {
        
    }
}