using AutoMapper.QueryableExtensions;
using Letterbook.Core;
using Letterbook.Core.Extensions;
using Letterbook.Core.Values;
using Medo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Letterbook.Web.Pages;

[AllowAnonymous]
[Authorize(Policy = Constants.AuthzPolicy)]
public class Profile : PageModel
{
	private readonly IProfileService _profileSvc;
	private readonly ILogger<Profile> _logger;
	private readonly CoreOptions _opts;
	private IAuthzProfileService _profiles;
	private Projections.Profile _profile;
	private Models.Profile? _self;

	/// The full conventional fediverse @user@domain username.
	/// Ostensibly globally unique
	public string FullHandle => $"@{BareHandle}@{_profile.Authority}";

	public Models.Profile? UserProfile => _self;

	/// Just the username, with no @'s
	public string BareHandle => _profile.Handle;

	/// The non-unique display name
	public string DisplayName => _profile.DisplayName;

	/// The profile bio
	public HtmlString Description => new(_profile.Description);

	public DateTimeOffset CreatedDate => _profile.Created;

	/// Public extra key-value fields
	public Models.CustomField[] CustomFields => _profile.CustomFields;

	/// The total number of profiles following this one
	public int FollowerCount => _profile.FollowersCount;

	/// The total number of profiles followed by this one
	public int FollowingCount => _profile.FollowingCount;

	public int PostCount => _profile.PostsCount;
	public IEnumerable<Models.Post> Posts => _profile.Posts;

	/// The internal unique ID for this profile (used a lot in our APIs)
	public string GetId => _profile.Id.ToString();

	public string? SelfId => User.Claims.FirstOrDefault(c => c.Type == "activeProfile")?.Value;

	/// Whether this profile follows the User's activeProfile
	public FollowState FollowsYou => _self?.FollowersCollection.FirstOrDefault(r => r.Follower.Id == _profile.Id)?.State ?? FollowState.None;

	/// Whether the User's activeProfile follows this one
	public FollowState YouFollow => _self?.FollowingCollection.FirstOrDefault(r => r.Follows.Id == _profile.Id)?.State ?? FollowState.None;

	public bool Blocked => YouFollow == FollowState.Blocked;


	public Profile(IProfileService profiles, ILogger<Profile> logger, IOptions<CoreOptions> opts)
	{
		_profile = default!;
		_profiles = default!;
		_profileSvc = profiles;
		_logger = logger;
		_opts = opts.Value;
	}

	public async Task<IActionResult> OnGet(string id, DateTimeOffset? postsBeforeDate = null)
	{
		_profiles = _profileSvc.As(User.Claims);

		if (id.StartsWith('@')) return await GetByHandle(id, postsBeforeDate ?? DateTimeOffset.MaxValue);
		return await GetById(id, postsBeforeDate ?? DateTimeOffset.MaxValue);
	}

	private async Task<IActionResult> GetById(string id, DateTimeOffset postsBefore)
	{
		if (!Models.ProfileId.TryParse(id, out var profileId)) return BadRequest();

		if (await _profiles.QueryProfiles(profileId)
			    .ProjectTo<Projections.Profile>(Projections.Profile.FromCoreModel(postsBefore))
			    .FirstOrDefaultAsync() is not { } profile)
			return NotFound();
		_profile = profile;

		await GetSelf();

		return Page();
	}

	public async Task<IActionResult> GetByHandle(string id, DateTimeOffset postsBefore)
	{
		var parts = id.Split("@", 2, StringSplitOptions.RemoveEmptyEntries);
		var handle = parts[0];
		var host = parts.Length == 2 ? parts[1] : _opts.BaseUri().GetAuthority();
		if (await _profiles.QueryProfiles(handle, host)
			    .ProjectTo<Projections.Profile>(Projections.Profile.FromCoreModel(postsBefore))
			    .FirstOrDefaultAsync() is not { } profile)
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