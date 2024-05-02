using Letterbook.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.Options;

namespace Letterbook.Web.Pages;

public class Profile : PageModel
{
	private readonly IProfileService _profiles;
	private readonly CoreOptions _options;

	public required string Handle { get; set; }
	public required string DisplayName { get; set; }
	public required HtmlString Description { get; set; }
	public required Models.CustomField[] CustomFields { get; set; }

	private protected Models.Profile? Prof { get; set; }

	public int GetFollowerCount() => Prof!.FollowersCollection.Count;
	public int GetFollowingCount() => Prof!.FollowingCollection.Count;


	public Profile(IProfileService profiles, IOptions<CoreOptions> options)
	{
		_profiles = profiles;
		_options = options.Value;
	}

	public async Task<IActionResult> OnGet(string handle)
	{
		var found = await _profiles.As(User.Claims).FindProfiles(handle);
		if (found.FirstOrDefault() is not { } profile)
			return NotFound();
		Prof = profile;

		Handle = $"@{handle}@{_options.DomainName}";
		DisplayName = profile.DisplayName;
		Description = new HtmlString(profile.Description);
		CustomFields = profile.CustomFields;


		return Page();
	}
}