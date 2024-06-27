using Letterbook.Core;
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
	private readonly IProfileService _profiles;
	private readonly CoreOptions _options;

	public string BareHandle { get; set; }
	public string Handle { get; set; }
	public string DisplayName { get; set; }
	public HtmlString Description { get; set; }
	public Models.CustomField[] CustomFields { get; set; }

	private protected Models.Profile? Prof { get; set; }

	public int GetFollowerCount() => Prof!.FollowersCollection.Count;
	public int GetFollowingCount() => Prof!.FollowingCollection.Count;
	public string GetId() => Prof!.GetId25();


	public Profile(IProfileService profiles, IOptions<CoreOptions> options)
	{
		_profiles = profiles;
		_options = options.Value;
		BareHandle = "";
		Handle = "";
		DisplayName = "";
		Description = new HtmlString("");
		CustomFields = new Models.CustomField[0];
	}

	public async Task<IActionResult> OnGet(string handle)
	{
		var found = await _profiles.As(User.Claims).FindProfiles(handle);
		if (found.FirstOrDefault() is not { } profile)
			return NotFound();
		Prof = profile;

		BareHandle = handle;
		Handle = $"@{handle}@{_options.DomainName}";
		DisplayName = profile.DisplayName;
		Description = new HtmlString(profile.Description);
		CustomFields = profile.CustomFields;

		return Page();
	}
}