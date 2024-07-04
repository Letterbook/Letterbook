using AutoMapper;
using Letterbook.Core;
using Letterbook.Core.Models.Dto;
using Letterbook.Core.Models.Mappers;
using Medo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;

namespace Letterbook.Web.Pages;

[AllowAnonymous]
[Authorize(Policy = Constants.AuthzPolicy)]
public class Profile : PageModel
{
	private readonly IProfileService _profileSvc;
	private readonly ILogger<Profile> _logger;
	private readonly CoreOptions _options;
	private readonly Mapper _mapper;
	private IAuthzProfileService _profiles;
	private Models.Profile _profile;
	private Models.Profile? _self;

	/// The full conventional fediverse @user@domain username.
	/// Ostensibly globally unique
	public string Handle => $"@{BareHandle}@{_profile.Authority}";

	/// Just the username, with no @'s
	public string BareHandle => _profile.Handle;

	/// The non-unique display name
	public string DisplayName => _profile.DisplayName;

	/// The profile bio
	public HtmlString Description => new(_profile.Description);

	/// Public extra key-value fields
	public Models.CustomField[] CustomFields => _profile.CustomFields;

	/// The total number of profiles following this one
	public Task<int> FollowerCount => _profiles.FollowerCount(_profile);

	/// The total number of profiles followed by this one
	public Task<int> FollowingCount => _profiles.FollowingCount(_profile);

	/// The internal unique ID for this profile (used a lot in our APIs)
	public string Id => _profile.GetId25();

	public string? SelfId => User.Claims.FirstOrDefault(c => c.Type == "activeProfile")?.Value;

	/// Whether this profile follows the User's activeProfile
	public bool FollowsYou => _self?.FollowersCollection.Any() ?? false;

	/// Whether the User's activeProfile follows this one
	public bool YouFollow => _self?.FollowingCollection.Any() ?? false;


	public Profile(IProfileService profiles, IOptions<CoreOptions> options, ILogger<Profile> logger, MappingConfigProvider maps)
	{
		_profile = default!;
		_profiles = default!;
		_profileSvc = profiles;
		_logger = logger;
		_mapper = new Mapper(maps.Profiles);
		_options = options.Value;
	}

	public async Task<IActionResult> OnGet(string handle)
	{
		_logger.LogDebug("Account {Name} has effective claims {Claims}", User.Identity?.Name, User.Claims);
		_profiles = _profileSvc.As(User.Claims);

		var found = await _profiles.FindProfiles(handle);
		if (found.FirstOrDefault() is not { } profile)
			return NotFound();

		_profile = profile;
		if (SelfId is { } id && await _profiles.LookupProfile(Uuid7.FromId25String(id), profile.GetId()) is { } self)
		{
			_self = self;
		}

		return Page();
	}

	public async Task<IActionResult> OnPostFollowRequest(Uuid7 followId)
	{
		if (SelfId is not { } selfId)
			return Challenge();

		_profiles = _profileSvc.As(User.Claims);
		var result = await _profiles.Follow(Uuid7.FromId25String(selfId), followId);
		_profile = result.Follows;
		_self = result.Follower;

		return Page();
	}

	public async Task<IActionResult> OnPostUnfollow(Uuid7 followId)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);
		if (SelfId is not { } selfId)
			return Challenge();

		_profiles = _profileSvc.As(User.Claims);
		var result = await _profiles.Unfollow(Uuid7.FromId25String(selfId), followId);
		_profile = (await _profiles.LookupProfile(followId))!;
		_self = (result?.Follower ?? await _profiles.LookupProfile(Uuid7.FromId25String(selfId)))!;

		return Page();
	}
}