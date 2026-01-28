using AutoMapper;
using Letterbook.Api.Swagger;
using Letterbook.Core;
using Letterbook.Core.Models;
using Letterbook.Core.Models.Dto;
using Letterbook.Core.Models.Mappers;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Letterbook.Api.Controllers;

[ApiExplorerSettings(GroupName = Docs.LetterbookV1)]
[Route("/lb/v1")]
public class WebFingerController : ControllerBase
{
	private readonly IProfileService _profiles;
	private readonly Mapper _mapper;

	public WebFingerController(
		IProfileService profiles,
		MappingConfigProvider mappingConfig)
	{
		_profiles = profiles;
		_mapper = new Mapper(mappingConfig.Profiles);
	}

	[HttpGet(".well-known/webfinger")]
	[ProducesResponseType<FullProfileDto>(StatusCodes.Status200OK)]
	[SwaggerOperation("Get", "Lookup a profile by web finger")]
	public async Task<IActionResult> GetByWebFinger([FromQuery(Name = "resource")]string[] resources /* acct:handle@authority */)
	{
		if (resources.Length != 1)
			return BadRequest();

		var resource = resources[0];

		if (string.IsNullOrEmpty(resource) || !AccountUri.TryParse(resource, out var accountUri))
			return BadRequest();

		var result = await _profiles.As(User.Claims).LookupProfile(accountUri!, null);

		return result != null ? Ok(_mapper.Map<FullProfileDto>(result)): NotFound();
	}
}