using Letterbook.Core;
using Letterbook.Core.Models.WebFinger;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Letterbook.Api.Controllers;

public class WebFingerController
{
    private readonly IAccountService _accountService;

    public WebFingerController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet]
    [Route("/.well-known/webfinger")]
    public ActionResult Get([FromQuery]string resource)
    {
        var profile = _accountService.LookupProfile(WebFingerQueryTarget.Parse(resource));

        return new OkObjectResult(new WebFingerJsonResourceDescriptor
        {
            Subject = $"acct:{profile.DisplayName}@{profile.Authority}"
        });
    }
}