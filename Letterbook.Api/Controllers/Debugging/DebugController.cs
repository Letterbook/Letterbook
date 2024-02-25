using Letterbook.Core;
using Letterbook.Core.Extensions;
using Medo;
using Microsoft.AspNetCore.Mvc;

namespace Letterbook.Api.Controllers.Debugging;

/// <summary>
/// A temporary controller to make it easier to trigger the actions we want to test
/// </summary>
public class DebugController : ControllerBase
{
    private readonly IProfileService _profileService;

    public DebugController(IProfileService profileService)
    {
        _profileService = profileService;
    }
    
    [HttpPost]
    [Route("{selfId}/follow/")]
    public async Task<IActionResult> DoFollow(string selfId, [FromBody]FollowRequest target)
    {
        Guid localId;
        try
        {
            localId = Uuid7.FromId25String(selfId);
        }
        catch (Exception)
        {
            return BadRequest();
        }

        var result = await _profileService.Follow(localId, new Uri(target.TargetId));

        return Ok(result);
    }
}

public class FollowRequest
{
    public required string TargetId { get; set; }
}