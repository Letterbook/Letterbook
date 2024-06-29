using AutoMapper;
using Letterbook.Api.Dto;
using Letterbook.Api.Swagger;
using Letterbook.Core;
using Letterbook.Core.Exceptions;
using Letterbook.Core.Models.Dto;
using Letterbook.Core.Models.Mappers;
using Medo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;
using IAuthorizationService = Letterbook.Core.IAuthorizationService;
using Models = Letterbook.Core.Models;

namespace Letterbook.Api.Controllers;

[Authorize(Policy = Constants.ApiPolicy)]
[ApiExplorerSettings(GroupName = Docs.LetterbookV1)]
[Route("/lb/v1/[controller]")]
public class ProfilesController : ControllerBase
{
	private readonly ILogger<ProfilesController> _logger;
	private readonly IOptions<CoreOptions> _options;
	private readonly IProfileService _profiles;
	private readonly IAuthorizationService _authz;
	private readonly Mapper _mapper;

	public ProfilesController(ILogger<ProfilesController> logger, IOptions<CoreOptions> options, IProfileService profiles, MappingConfigProvider mappingConfig, IAuthorizationService authz)
	{
		_logger = logger;
		_options = options;
		_profiles = profiles;
		_authz = authz;
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

	[HttpPost("new/{accountId}")]
	[ProducesResponseType<FullProfileDto>(StatusCodes.Status200OK)]
	[SwaggerOperation("New Profile", "Create a new profile that belongs to the given account")]
	public async Task<IActionResult> Create(Guid accountId, [FromQuery]string handle)
	{
		var result = await _profiles.As(User.Claims).CreateProfile(accountId, handle);
		return Ok(_mapper.Map<FullProfileDto>(result));
	}

	[HttpDelete("{profileId}")]
	[ProducesResponseType<FullProfileDto>(StatusCodes.Status200OK)]
	[SwaggerOperation("Delete", "Not Implemented")]
	public async Task<IActionResult> Delete(Uuid7 profileId)
	{
		await Task.CompletedTask;
		throw new NotImplementedException();
	}

	[HttpPut("{profileId}")]
	[ProducesResponseType<FullProfileDto>(StatusCodes.Status200OK)]
	[SwaggerOperation("Edit", "Update profile properties")]
	public async Task<IActionResult> Edit(Uuid7 profileId, [FromBody]FullProfileDto dto)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);
		if (_mapper.Map<Models.Profile>(dto) is not {} profile)
			return BadRequest(new ErrorMessage(ErrorCodes.InvalidRequest, $"Invalid {typeof(FullProfileDto)}"));

		var decision = _authz.Update(User.Claims, profile, profileId);
		if (!decision.Allowed)
			return Unauthorized(decision);

		var result = await _profiles.As(User.Claims).UpdateProfile(profile);
		return result.Updated != null
			? Ok(_mapper.Map<FullProfileDto>(result.Updated))
			: NotFound();
	}

	[HttpPost("{profileId}/field/{index}")]
	[ProducesResponseType<FullProfileDto>(StatusCodes.Status200OK)]
	[SwaggerOperation("Add Field", "Add a custom field to the profile")]
	public async Task<IActionResult> AddField(Uuid7 profileId, int index, [FromBody]Models.CustomField dto)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		var result = await _profiles.As(User.Claims).InsertCustomField(profileId, index, dto.Label, dto.Value);
		return result.Updated != null
			? Ok(_mapper.Map<FullProfileDto>(result.Updated))
			: NotFound();
	}

	[HttpDelete("{profileId}/field/{index}")]
	[ProducesResponseType<FullProfileDto>(StatusCodes.Status200OK)]
	[SwaggerOperation("Remove Field", "Remove a custom field from the profile")]
	public async Task<IActionResult> RemoveField(Uuid7 profileId, int index)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		var result = await _profiles.As(User.Claims).RemoveCustomField(profileId, index);
		return result.Updated != null
			? Ok(_mapper.Map<FullProfileDto>(result.Updated))
			: NotFound();
	}

	[HttpPut("{profileId}/field/{index}")]
	[ProducesResponseType<FullProfileDto>(StatusCodes.Status200OK)]
	[SwaggerOperation("Update Field", "Update a custom field on the profile")]
	public async Task<IActionResult> UpdateField(Uuid7 profileId, int index, [FromBody]Models.CustomField dto)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		var result = await _profiles.As(User.Claims).UpdateCustomField(profileId, index, dto.Label, dto.Value);
		return result.Updated != null
			? Ok(_mapper.Map<FullProfileDto>(result.Updated))
			: NotFound();
	}

	[HttpGet("{profileId}/follow")]
	[ProducesResponseType<IEnumerable<MiniProfileDto>>(StatusCodes.Status200OK)]
	[SwaggerOperation("Get followed profiles", "Get the paged list of profiles followed by the given profile")]
	public async Task<IActionResult> GetFollowing(Uuid7 profileId, DateTimeOffset? followedBefore, int limit = 100)
	{
		followedBefore ??= DateTimeOffset.UtcNow;
		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		var list = await _profiles.As(User.Claims).LookupFollowing(profileId, followedBefore, limit);
		return Ok(list.Select(_mapper.Map<MiniProfileDto>));
	}


	[HttpPut("{profileId}/follow/{followProfileId}")]
	[ProducesResponseType<FollowerRelationDto>(StatusCodes.Status200OK)]
	[SwaggerOperation("Follow request", "Request to follow the followProfileId")]
	public async Task<IActionResult> Follow(Uuid7 profileId, Uuid7 followProfileId)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		var result = await _profiles.As(User.Claims).Follow(profileId, followProfileId);
		return Ok(result);
	}

	[HttpDelete("{profileId}/follow/{followProfileId}")]
	[ProducesResponseType<FollowerRelationDto>(StatusCodes.Status200OK)]
	[SwaggerOperation("Unfollow", "Stop following the followProfileId")]
	public async Task<IActionResult> Unfollow(Uuid7 profileId, Uuid7 followProfileId)
	{
		await Task.CompletedTask;
		throw new NotImplementedException();
	}

	[HttpGet("{profileId}/follower")]
	[ProducesResponseType<IEnumerable<MiniProfileDto>>(StatusCodes.Status200OK)]
	[SwaggerOperation("Get followers", "Get the paged list of profiles that follow the given profile")]
	public async Task<IActionResult> GetFollowers(Uuid7 profileId, DateTimeOffset? followedBefore, int limit = 100)
	{
		followedBefore ??= DateTimeOffset.UtcNow;
		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		var list = await _profiles.As(User.Claims).LookupFollowers(profileId, followedBefore, limit);
		return Ok(list.Select(_mapper.Map<MiniProfileDto>));
	}

	[HttpPut("{profileId}/follower/{followerProfileId}")]
	[ProducesResponseType<FollowerRelationDto>(StatusCodes.Status200OK)]
	[SwaggerOperation("Accept follow request", "Accept a follow request from the followerProfileId")]
	public async Task<IActionResult> AcceptFollower(Uuid7 profileId, Uuid7 followerProfileId)
	{
		await Task.CompletedTask;
		throw new NotImplementedException();
	}


	[HttpDelete("{profileId}/follower/{followerProfileId}")]
	[ProducesResponseType<FollowerRelationDto>(StatusCodes.Status200OK)]
	[SwaggerOperation("Remove Follower", "Remove the followProfileId as a follower, or reject their follow request")]
	public async Task<IActionResult> RemoveFollower(Uuid7 profileId, Uuid7 followerProfileId)
	{
		await Task.CompletedTask;
		throw new NotImplementedException();
	}
}