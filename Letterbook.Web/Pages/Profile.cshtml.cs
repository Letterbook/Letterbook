using Letterbook.Core;
using Models = Letterbook.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.Web.Pages;

public class Profile : PageModel
{
	private readonly IProfileService _profiles;
	
	public required string Handle { get; set; }
	public required string DisplayName { get; set; }
	public required string Description { get; set; }
	public required Models.CustomField[] CustomFields { get; set; }
	
	private protected Models.Profile? Prof { get; set; }
	
	public int GetFollowerCount => Prof!.FollowersCollection.Count;
	public int GetFollowingCount => Prof!.FollowingCollection.Count;


	public Profile(IProfileService profiles)
	{
		_profiles = profiles;
	}

	public async Task<IActionResult> OnGet(string handle)
	{
		var found = await _profiles.As(User.Claims).FindProfiles(handle);
		if (found.FirstOrDefault() is not { } profile)
			return NotFound();
		Prof = profile;
		Handle = handle;
		DisplayName = profile.DisplayName;
		Description = profile.Description;
		CustomFields = profile.CustomFields;


		return Page();
	}
}