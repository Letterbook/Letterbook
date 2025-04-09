using Letterbook.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace Letterbook.Web.Pages;

[Authorize(Policy = Constants.AuthzPolicy)]
public class ProfileEdit : PageModel
{
    private readonly IProfileService _profiles;
	private readonly CoreOptions _options;

   	public string Handle { get; set; }

	public ProfileEdit(IProfileService profiles, IOptions<CoreOptions> options)
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
			return RedirectToPage("Profile", new { handle = profile.Handle });
		}
		return Page();
	}
}