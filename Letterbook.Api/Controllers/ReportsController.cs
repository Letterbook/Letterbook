using AutoMapper;
using Letterbook.Api.Dto;
using Letterbook.Api.Swagger;
using Letterbook.Core;
using Letterbook.Core.Exceptions;
using Letterbook.Core.Models;
using Letterbook.Core.Models.Dto;
using Letterbook.Core.Models.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using IAuthorizationService = Letterbook.Core.IAuthorizationService;

namespace Letterbook.Api.Controllers;

[Authorize(Policy = Constants.ApiPolicy)]
[ApiExplorerSettings(GroupName = Docs.LetterbookV1)]
[Route("lb/v1/[controller]")]
public class ReportsController : ControllerBase
{
	private readonly IMapper _mapper;
	private readonly IModerationService _moderation;
	private readonly IAuthorizationService _authz;

	public ReportsController(MappingConfigProvider mappingConfig, IModerationService moderation, IAuthorizationService authz)
	{
		_moderation = moderation;
		_authz = authz;
		_mapper = new Mapper(mappingConfig.Posts);
	}

	[HttpPost("{profileId}/post")]
	[ProducesResponseType<PostDto>(StatusCodes.Status200OK)]
	[SwaggerOperation("Post", "Open a new moderation report")]
	public async Task<IActionResult> CreateReport(ProfileId selfId, [FromBody] MemberModerationReportDto reportDto)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);
		reportDto.Id = default;

		if (reportDto.Reporter != selfId)
			throw CoreException.InvalidRequest($"Cannot create report as Profile {reportDto.Reporter}");
		if (_mapper.Map<ModerationReport>(reportDto) is not { } report)
			return BadRequest(new ErrorMessage(ErrorCodes.InvalidRequest, $"Invalid {typeof(MemberModerationReportDto)}"));

		var result = await _moderation.As(User.Claims).CreateReport(selfId, report);

		return Ok(_authz.Update(User.Claims, result)
			? _mapper.Map<FullModerationReportDto>(result)
			: _mapper.Map<MemberModerationReportDto>(result));
	}
}