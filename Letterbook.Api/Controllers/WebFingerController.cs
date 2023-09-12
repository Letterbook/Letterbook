using Letterbook.Core;
using Microsoft.AspNetCore.Mvc;

namespace Letterbook.Api.Controllers;

public class WebFingerController : ControllerBase
{
    private readonly IAccountService _accountService;

    public WebFingerController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet]
    [Route("/.well-known/webfinger")]
    public async Task<IResult> Get([FromQuery]string resource)
    {
        var profile = await _accountService.LookupProfile(resource);

        return Results.Ok(new WebFingerJsonResourceDescriptor
        {
            Subject = $"acct:{profile.DisplayName}@{profile.Authority}"
        });
    }
}