﻿using Letterbook.Core;
using Letterbook.Core.Extensions;
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
	private readonly CoreOptions _opts;
	private IAuthzProfileService _profiles;
	private Models.Profile _profile;
	private Models.Profile? _self;

	/// The full conventional fediverse @user@domain username.
	/// Ostensibly globally unique
	public string FullHandle => $"@{BareHandle}@{_profile.Authority}";

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
	public string GetId => _profile.GetId25();

	public string? SelfId => User.Claims.FirstOrDefault(c => c.Type == "activeProfile")?.Value;

	/// Whether this profile follows the User's activeProfile
	public bool FollowsYou => _self?.FollowersCollection.Any() ?? false;

	/// Whether the User's activeProfile follows this one
	public bool YouFollow => _self?.FollowingCollection.Any() ?? false;


	public Profile(IProfileService profiles, ILogger<Profile> logger, IOptions<CoreOptions> opts)
	{
		_profile = default!;
		_profiles = default!;
		_profileSvc = profiles;
		_logger = logger;
		_opts = opts.Value;
	}

	public async Task<IActionResult> OnGet(string id)
	{
		_profiles = _profileSvc.As(User.Claims);

		if (id.StartsWith('@')) return await GetByHandle(id);
		return await GetById(id);
	}

	private async Task<IActionResult> GetById(string id)
	{
		if (!Models.ProfileId.TryParse(id, out var profileId)) return BadRequest();

		if (await _profiles.LookupProfile(profileId) is not { } profile)
			return NotFound();
		_profile = profile;

		await GetSelf();

		return Page();
	}

	public async Task<IActionResult> GetByHandle(string id)
	{
		var parts = id.Split("@", 2, StringSplitOptions.RemoveEmptyEntries);
		var handle = parts[0];
		var host = parts.Length == 2 ? parts[1] : _opts.BaseUri().GetAuthority();
		if (await _profiles.FindProfiles(handle, host).FirstOrDefaultAsync() is not { } profile)
			return NotFound();
		_profile = profile;

		await GetSelf();

		return Page();
	}

	private async Task GetSelf()
	{
		if (SelfId is { } selfId && await _profiles.LookupProfile(Models.ProfileId.FromString(selfId), _profile.Id) is { } self)
		{
			_self = self;
		}
	}

	public async Task<IActionResult> OnPostFollowRequest(string handle, Uuid7 followId)
	{
		if (SelfId is not { } selfId)
			return Challenge();

		_profiles = _profileSvc.As(User.Claims);
		await _profiles.Follow(Uuid7.FromId25String(selfId), followId);

		return RedirectToPage(GetType().Name, new { handle });
	}

	public async Task<IActionResult> OnPostUnfollow(string handle, Uuid7 followId)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);
		if (SelfId is not { } selfId)
			return Challenge();

		_profiles = _profileSvc.As(User.Claims);
		await _profiles.Unfollow(Uuid7.FromId25String(selfId), followId);

		return RedirectToPage(GetType().Name, new { handle });
	}

	public async Task<IActionResult> OnPostRemoveFollower(string handle, Uuid7 followerId)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);
		if (SelfId is not { } selfId)
			return Challenge();

		_profiles = _profileSvc.As(User.Claims);
		await _profiles.RemoveFollower(Uuid7.FromId25String(selfId), followerId);

		return RedirectToPage(GetType().Name, new { handle });
	}
}