using Microsoft.AspNetCore.Mvc;

namespace Letterbook.Api.Controllers.Mastodon;

[Route("api/v1/[controller]")]
public class AccountsController
{
    [HttpPost]
    public IActionResult RegisterAccount()
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    [Route("[action]")]
    public IActionResult VerifyCredentials()
    {
        throw new NotImplementedException();
    }
    
    
    
    [HttpGet]
    [Route("{id}/[action]")]
    public IActionResult Statuses(int id)
    {
        throw new NotImplementedException();
    }
    
    [HttpGet]
    [Route("{id}")]
    public IActionResult GetAccount(int id)
    {
        throw new NotImplementedException();

    }

    [HttpGet]
    [Route("{id}/[action]")]
    public IActionResult ScheduledStatuses(int id)
    {
        throw new NotImplementedException();

    }

    [HttpPatch]
    [Route("[action]")]
    public IActionResult UpdateCredentials( )
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    [Route("{id}/[action]")]
    public IActionResult Followers(int id)
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    [Route("{id}/[action]")]
    public IActionResult Following(int id)
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    [Route("{id}/[action]")]
    public IActionResult FeaturedTags(int id)
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    [Route("{id}/[action]")]
    public IActionResult Lists(int id)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    [Route("{id}/[action]")]
    public IActionResult Follow(int id)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    [Route("{id}/[action]")]
    public IActionResult Unfollow(int id)
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    [Route("{id}/[action]")]
    public IActionResult RemoveFromFollowers(int id)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    [Route("{id}/[action]")]
    public IActionResult Block(int id)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    [Route("{id}/[action]")]
    public IActionResult Unblock(int id)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    [Route("{id}/[action]")]
    public IActionResult Mute(int id)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    [Route("{id}/[action]")]
    public IActionResult Unmute(int id)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    [Route("{id}/[action]")]
    public IActionResult Pin(int id)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    [Route("{id}/[action]")]
    public IActionResult Unpin(int id)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    [Route("{id}/[action]")]
    public IActionResult Note(int id)
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    [Route("[action]")]
    public IActionResult Relationships(int[] ids)
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    [Route("[action]")]
    public IActionResult FamiliarFollowers(int[] ids)
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    [Route("[action]")]
    public IActionResult Search(string q, int limit, int offset, bool resolve, bool following )
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    [Route("[action]")]
    public IActionResult Lookup(string acct )
    {
        throw new NotImplementedException();
    }
}
