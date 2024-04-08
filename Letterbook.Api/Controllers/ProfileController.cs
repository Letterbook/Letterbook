using AutoMapper;
using Letterbook.Api.Dto;
using Letterbook.Api.Mappers;
using Letterbook.Api.Swagger;
using Letterbook.Core;
using Medo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;

namespace Letterbook.Api.Controllers;

[ApiExplorerSettings(GroupName = Docs.LetterbookV1)]
[Route("/lb/v1/[controller]/[action]")]
public class ProfileController : ControllerBase
{
	private readonly ILogger<ProfileController> _logger;
	private readonly IOptions<CoreOptions> _options;
	private readonly IProfileService _profiles;
	private readonly Mapper _mapper;

	public ProfileController(ILogger<ProfileController> logger, IOptions<CoreOptions> options, IProfileService profiles, MappingConfigProvider mappingConfig)
	{
		_logger = logger;
		_options = options;
		_profiles = profiles;
		_mapper = new Mapper(mappingConfig.Profiles);
	}

	[HttpGet("{profileId}")]
	[ProducesResponseType<FullProfileDto>(StatusCodes.Status200OK)]
	[SwaggerOperation("Get", "Lookup a profile by ID")]
	public async Task<IActionResult> Get(Uuid7 profileId)
	{
		var result = await _profiles.As(User.Claims).LookupProfile(profileId);
		return result != null
			? Ok(_mapper.Map<FullProfileDto>(result))
			: NotFound();
	}

	[HttpPost("new/account/{accountId}")]
	[ProducesResponseType<FullProfileDto>(StatusCodes.Status200OK)]
	[SwaggerOperation("New Profile", "Create a new profile that belongs to the given account")]
	public async Task<IActionResult> Create(Guid accountId, [FromQuery]string handle)
	{
		var result = await _profiles.As(User.Claims).CreateProfile(accountId, handle);
		return Ok(_mapper.Map<FullProfileDto>(result));
	}

	// create
	// delete
	// edit
	// add field
	// remove field
	// edit field

}