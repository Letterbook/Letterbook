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
using Microsoft.IdentityModel.JsonWebTokens;
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
		_mapper = new Mapper(mappingConfig.ModerationReports);
	}

	private bool TryGetAccountId(out Guid accountId)
	{
		return Guid.TryParse(User.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value, out accountId);
	}

	[HttpPost("{selfId}/report")]
	[ProducesResponseType<FullModerationReportDto>(StatusCodes.Status200OK)]
	[ProducesResponseType<MemberModerationReportDto>(StatusCodes.Status200OK)]
	[SwaggerOperation("Create", "Open a new moderation report")]
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

	[HttpGet("{selfId}/report/{reportId}")]
	[ProducesResponseType<FullModerationReportDto>(StatusCodes.Status200OK)]
	[ProducesResponseType<MemberModerationReportDto>(StatusCodes.Status200OK)]
	[SwaggerOperation("Get", "Get a new moderation report by ID")]
	public async Task<IActionResult> LookupReport(ProfileId selfId, ModerationReportId reportId)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);
		if (!_authz.Any(User.Claims, selfId))
			return Unauthorized();

		var result = await _moderation.As(User.Claims).LookupReport(reportId);

		return Ok(_authz.Update(User.Claims, result)
			? _mapper.Map<FullModerationReportDto>(result)
			: _mapper.Map<MemberModerationReportDto>(result));
	}

	[HttpGet("moderator/report")]
	[ProducesResponseType<IAsyncEnumerable<FullModerationReportDto>>(StatusCodes.Status200OK)]
	[SwaggerOperation("List", "Filter and list moderation reports")]
	public IActionResult ListReports([FromQuery] ProfileId? subjectId = null, [FromQuery] ProfileId? reporterId = null,
		[FromQuery] Guid? moderatorId = null, [FromQuery] bool includeClosed = false)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);
		if (!_authz.Update<ModerationReport>(User.Claims))
			return Unauthorized();

		if (subjectId is { } subject)
			return Ok(_moderation.As(User.Claims).FindRelatedTo(subject, includeClosed).Select(_mapper.Map<FullModerationReportDto>));
		if (reporterId is { } reporter)
			return Ok(_moderation.As(User.Claims).FindCreatedBy(reporter, includeClosed).Select(_mapper.Map<FullModerationReportDto>));
		if (moderatorId is { } moderator)
			return Ok(_moderation.As(User.Claims).FindAssigned(moderator, includeClosed).Select(_mapper.Map<FullModerationReportDto>));

		return BadRequest();
	}

	[HttpPut("moderator/report/{reportId}")]
	[ProducesResponseType<FullModerationReportDto>(StatusCodes.Status200OK)]
	[ProducesResponseType<MemberModerationReportDto>(StatusCodes.Status200OK)]
	[SwaggerOperation("Update", "Update a moderation report")]
	public async Task<IActionResult> UpdateReport(ModerationReportId reportId, [FromBody] FullModerationReportDto dto)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);
		if (!TryGetAccountId(out var accountId))
			return Forbid();

		if (_mapper.Map<ModerationReport>(dto) is not { } report)
			return BadRequest(new ErrorMessage(ErrorCodes.InvalidRequest, $"Invalid {typeof(MemberModerationReportDto)}"));

		var result = await _moderation.As(User.Claims).UpdateReport(reportId, report, accountId);

		return Ok(_authz.Update(User.Claims, result)
			? _mapper.Map<FullModerationReportDto>(result)
			: _mapper.Map<MemberModerationReportDto>(result));
	}

	[HttpPost("moderator/report/{reportId}/remark")]
	[ProducesResponseType<FullModerationReportDto>(StatusCodes.Status200OK)]
	[ProducesResponseType<MemberModerationReportDto>(StatusCodes.Status200OK)]
	[SwaggerOperation("Remark", "Add a remark to a moderation report")]
	public async Task<IActionResult> Remark(ModerationReportId reportId, [FromBody] ModerationRemarkDto dto)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		if (_mapper.Map<ModerationRemark>(dto) is not { } remark)
			return BadRequest(new ErrorMessage(ErrorCodes.InvalidRequest, $"Invalid {typeof(MemberModerationReportDto)}"));

		var result = await _moderation.As(User.Claims).AddRemark(reportId, remark);

		return Ok(_authz.Update(User.Claims, result)
			? _mapper.Map<FullModerationReportDto>(result)
			: _mapper.Map<MemberModerationReportDto>(result));
	}

	[HttpPut("moderator/report/{reportId}/assign/{accountId}")]
	[ProducesResponseType<FullModerationReportDto>(StatusCodes.Status200OK)]
	[ProducesResponseType<MemberModerationReportDto>(StatusCodes.Status200OK)]
	[SwaggerOperation("Assign", "Assign or unassign a moderator")]
	public async Task<IActionResult> Assign(ModerationReportId reportId, Guid accountId, [FromQuery] bool unassign = false)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		var result = await _moderation.As(User.Claims).AssignModerator(reportId, accountId, unassign);

		return Ok(_authz.Update(User.Claims, result)
			? _mapper.Map<FullModerationReportDto>(result)
			: _mapper.Map<MemberModerationReportDto>(result));
	}

	[HttpPut("moderator/report/{reportId}/close")]
	[ProducesResponseType<FullModerationReportDto>(StatusCodes.Status200OK)]
	[ProducesResponseType<MemberModerationReportDto>(StatusCodes.Status200OK)]
	[SwaggerOperation("Close", "Close or reopen a moderation report")]
	public async Task<IActionResult> Close(ModerationReportId reportId, [FromQuery] bool reopen = false)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		if (!TryGetAccountId(out var accountId)) return Forbid();
		var result = await _moderation.As(User.Claims).CloseReport(reportId, accountId, reopen);

		return Ok(_authz.Update(User.Claims, result)
			? _mapper.Map<FullModerationReportDto>(result)
			: _mapper.Map<MemberModerationReportDto>(result));
	}
}