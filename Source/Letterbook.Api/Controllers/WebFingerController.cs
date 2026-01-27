using AutoMapper;
using Letterbook.Api.Swagger;
using Letterbook.Core;
using Letterbook.Core.Models.Dto;
using Letterbook.Core.Models.Mappers;
using MassTransit.Testing;
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

	// https://mastodon.social/.well-known/webfinger?resource=acct:gargron@mastodon.social
	[HttpGet(".well-known/webfinger")]
	[ProducesResponseType<FullProfileDto>(StatusCodes.Status200OK)]
	[SwaggerOperation("Get", "Lookup a profile by web finger")]
	public async Task<IActionResult> GetByWebFinger([FromQuery]string resource)
	{
		var result = await _profiles.As(User.Claims).FindProfiles(resource).FirstOrDefaultAsync();

		return result != null ? Ok(_mapper.Map<FullProfileDto>(result)): NotFound();
	}
}