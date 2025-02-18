using AutoMapper;
using Letterbook.Api.Swagger;
using Letterbook.Core;
using Letterbook.Core.Models;
using Letterbook.Core.Models.Dto;
using Letterbook.Core.Models.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using IAuthorizationService = Letterbook.Core.IAuthorizationService;

namespace Letterbook.Api;

[Authorize(Policy = Constants.ApiPolicy)]
[ApiExplorerSettings(GroupName = Docs.LetterbookV1)]
[Route("lb/v1/[controller]")]
public class PoliciesController : ControllerBase
{
	private readonly IModerationService _moderation;
	private readonly IAuthorizationService _authz;
	private readonly Mapper _mapper;

	public PoliciesController(MappingConfigProvider mappingConfig, IModerationService moderation, IAuthorizationService authz)
	{
		_moderation = moderation;
		_authz = authz;
		_mapper = new Mapper(mappingConfig.ModerationReports);
	}

	[HttpGet("public/policy")]
	[ProducesResponseType<IEnumerable<ModerationPolicyDto>>(StatusCodes.Status200OK)]
	[SwaggerOperation("Get", "List moderation policies")]
	public IActionResult LookupReport(ModerationPolicyId policyId, bool includeRetired = false)
	{
		return Ok(_moderation.As(User.Claims).ListPolicies(includeRetired).Select(_mapper.Map<ModerationPolicyDto>));
	}

	[HttpPost("moderator/policy")]
	[ProducesResponseType<ModerationPolicyDto>(StatusCodes.Status200OK)]
	[SwaggerOperation("Add a policy", "Add a new moderation policies")]
	public async Task<IActionResult> AddPolicy([FromBody]ModerationPolicy policy)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		var result = await _moderation.As(User.Claims).AddPolicy(policy);
		return Ok(_mapper.Map<ModerationPolicyDto>(result));
	}

	[HttpPut("moderator/policy/{policyId}")]
	[ProducesResponseType<ModerationPolicyDto>(StatusCodes.Status200OK)]
	[SwaggerOperation("Retire a policy", "Retire an existing moderation policies")]
	public async Task<IActionResult> RetirePolicy(ModerationPolicyId policyId, bool restore = false)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		var result = await _moderation.As(User.Claims).RetirePolicy(policyId, restore);
		return Ok(_mapper.Map<ModerationPolicyDto>(result));
	}
}