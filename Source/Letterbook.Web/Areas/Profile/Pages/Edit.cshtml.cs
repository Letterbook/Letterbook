using System.ComponentModel.DataAnnotations;
using Letterbook.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Letterbook.Web.Areas.Profile.Pages;

[Authorize(Policy = Constants.AuthzPolicy)]
public class Edit : PageModel
{
    private readonly IProfileService _profiles;
	private readonly CoreOptions _options;

   	public string Handle { get; set; }

	public Edit(IProfileService profiles, IOptions<CoreOptions> options)
	{
		_profiles = profiles;
		_options = options.Value;
		Description = "";
		DisplayName = "";
		Handle = "";
	}

	[BindProperty]
	[Required]
	[StringLength(60)]
	public string DisplayName { get; set; }

	[BindProperty]
	[StringLength(1000)]
	public string Description { get; set; }

    public async Task<IActionResult> OnGet(Models.ProfileId id)
    {
	    var profile = await _profiles.As(User.Claims).LookupProfile(id);
		if (profile == null)
			return NotFound();

		Handle = profile.Handle;
		DisplayName = profile.DisplayName;
		Description = profile.Description;
        return Page();
    }

	public async Task<IActionResult> OnPostAsync(Models.ProfileId id)
	{
		var profile = await _profiles.As(User.Claims).LookupProfile(id);
		if (profile == null)
			return NotFound();

		if (ModelState.IsValid) {
			await _profiles.As(User.Claims).UpdateDisplayName(profile.Id, DisplayName);
			await _profiles.As(User.Claims).UpdateDescription(profile.Id, Description);
			// RedirectToPage
			return RedirectToPage("Profile", new { id = profile.Handle });
		}

		return Page();
	}
}