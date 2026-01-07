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
		CustomFields = [];
	}

	[BindProperty]
	[Required]
	[StringLength(60)]
	public string DisplayName { get; set; }

	[BindProperty]
	[StringLength(1000)]
	public string Description { get; set; }

	[BindProperty]
	public Models.ProfileId Id { get; set; }

	[BindProperty]
	public Models.CustomField[] CustomFields { get; set; }

	public async Task<IActionResult> OnGet(Models.ProfileId id)
    {
	    var profile = await _profiles.As(User.Claims).LookupProfile(id);
		if (profile == null)
			return NotFound();

		Handle = profile.Handle;
		DisplayName = profile.DisplayName;
		Description = profile.Description;
		CustomFields = new Models.CustomField[_options.MaxCustomFields];
		for (var i = 0; i < profile.CustomFields.Length && i < CustomFields.Length; i++)
		{
			CustomFields[i] = profile.CustomFields[i];
		}
		Id = profile.Id;
        return Page();
    }

	public async Task<IActionResult> OnPostAsync()
	{
		var profile = await _profiles.As(User.Claims).LookupProfile(Id);
		if (profile == null)
			return NotFound();

		if (!ModelState.IsValid) return Page();

		profile.CustomFields = CustomFields;
		profile.DisplayName = DisplayName;
		profile.Description = Description;
		await _profiles.As(User.Claims).UpdateProfile(profile);

		// RedirectToPage
		return RedirectToPage("Profile", new { id = profile.Handle });

	}
}