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

    public async Task<IActionResult> OnGet(string handle)
    {
        var found = await _profiles.As(User.Claims).FindProfiles(handle).FirstOrDefaultAsync();
		if (found is null)
			return NotFound();

		Handle = handle;
		DisplayName = found.DisplayName;
		Description = found.Description;
        return Page();
    }

	public async Task<IActionResult> OnPostAsync(string handle)
	{
		var found = await _profiles.As(User.Claims).FindProfiles(handle).FirstOrDefaultAsync();
		if (found is null)
			return NotFound();

		if (ModelState.IsValid) {
			await _profiles.As(User.Claims).UpdateDisplayName(found.Id, DisplayName);
			await _profiles.As(User.Claims).UpdateDescription(found.Id, Description);
			return RedirectToPage("Profile", new { handle = handle });
		}
		return Page();
	}
}