using Letterbook.Api.Swagger;
using Letterbook.Core;
using Medo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Letterbook.Api.Controllers;

[ApiExplorerSettings(GroupName = Docs.LetterbookV1)]
[Route("/lb/v1/[controller]/[action]")]
public class ProfileController : ControllerBase
{
	private readonly ILogger<ProfileController> _logger;
	private readonly IOptions<CoreOptions> _options;
	private readonly IProfileService _profiles;

	public ProfileController(ILogger<ProfileController> logger, IOptions<CoreOptions> options, IProfileService profiles)
	{
		_logger = logger;
		_options = options;
		_profiles = profiles;
	}

	// get
	[HttpGet("{profileId}")]
	public async Task<IActionResult> Get(Uuid7 profileId)
	{
		var result = await _profiles.As(User.Claims).LookupProfile(profileId);
		return result != null
			? Ok(result)
			: NotFound();
	}
	// create
	// delete
	// edit
	// add field
	// remove field
	// edit field

}